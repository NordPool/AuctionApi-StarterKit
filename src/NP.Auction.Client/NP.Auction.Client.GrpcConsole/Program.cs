namespace NP.Auction.Client.GrpcConsole;

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using NP.Auction.Client.GrpcConsole.Helpers;
using NPS.Auction.API.gRPC.Protos;


public class Program
{
    private static readonly IConfiguration Config = ConfigurationHelper.GetConfiguration();

    private static async Task Main()
    {
        Console.Title = "Nord Pool Auction gRPC";

        var ssoClient = ConfigurationHelper.InitializeSsoClient(Config);
        var channelOptions = ConfigurationHelper.InitializeChannelOptions(Config, ssoClient);
        var url = Config.GetValue<string>("Settings:AuctionApiUrl");

        using var channel = GrpcChannel.ForAddress(url, channelOptions);
        
        var menuSelection = CommandType.None;

        while (menuSelection != CommandType.Quit)
        {
            menuSelection = ReadUserSelection();

            switch (menuSelection)
            {
                case CommandType.OpenAuctionStatusStream:
                    await StartAuctionStatusStream(channel);
                    break;
                case CommandType.OpenReasonabilityCheckResultsStream:
                    await StartReasonabilityCheckResultsStream(channel);
                    break;
                case CommandType.Quit:
                    break;
            }
        }
    }

    private static CommandType ReadUserSelection()
    {
        ConsoleHelper.WriteLine("Options:", ConsoleColor.DarkGreen);
        Console.WriteLine("1: Connect to Auction Status Stream");
        Console.WriteLine("2: Connect to Reasonability Check Results Stream");
        Console.WriteLine("Q: Quit");

        while (true)
        {
            Console.Write(">> ");
            var selection = Console.ReadKey();
            Console.WriteLine();

            switch (selection.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return CommandType.OpenAuctionStatusStream;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return CommandType.OpenReasonabilityCheckResultsStream;
                case ConsoleKey.Q:
                    return CommandType.Quit;
                default:
                    Console.WriteLine($"Invalid option - {selection.KeyChar}");
                    break;
            }
        }
    }

    private static async Task StartAuctionStatusStream(GrpcChannel channel)
    {
        var source = new CancellationTokenSource();
        
        var streamTask = Task.Run(function: async () =>
        {
            var retryCount = 0;
            var notifiedListeningStatus = false;
            var client = new AuctionNotifier.AuctionNotifierClient(channel);

            Console.Clear();
            ConsoleHelper.WriteLine("Connecting to stream - Press any key to disconnect", ConsoleColor.DarkGreen);

            while (true)
            {
                using var call = client.GetAuctionStatusStream(new Empty(), cancellationToken: source.Token);

                try
                {
                    await foreach (var response in call.ResponseStream.ReadAllAsync(source.Token))
                    {
                        if (!notifiedListeningStatus)
                        {
                            ConsoleHelper.WriteTimestampedOutput("Listening for Auction status updates", ConsoleColor.DarkCyan);
                            retryCount = 0;
                            notifiedListeningStatus = true;
                        }

                        if (response.HasStatus)
                        {
                            ConsoleHelper.WriteTimestampedOutput($"{response.AuctionStatus}");
                        }
                    }
                }
                catch (RpcException ex)
                {
                    switch (ex)
                    {
                        case { StatusCode: StatusCode.Unavailable }:
                            notifiedListeningStatus = false;
                            retryCount = await AttemptConnectionRetry(retryCount, ex, source);
                            break;
                        case { StatusCode: StatusCode.Cancelled }:
                            ConsoleHelper.WriteTimestampedOutput("Stream disconnected.", ConsoleColor.DarkCyan);
                            ConsoleHelper.WriteLine(new string('-', Console.WindowWidth));
                            return;
                        default:
                            source.Cancel();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
                    source.Cancel();
                    return;
                }
            }
        });

        while (!Console.KeyAvailable && !streamTask.IsCompleted)
        {
            await Task.Delay(250);
        }

        if (!streamTask.IsCompleted)
        {
            Console.ReadKey(true);
            source.Cancel();
            await streamTask;
        }
        
    }
    private static async Task StartReasonabilityCheckResultsStream(GrpcChannel channel)
    {
        var source = new CancellationTokenSource();
        
        var streamTask = Task.Run(function: async () =>
        {
            var retryCount = 0;
            var notifiedListeningStatus = false;
            var client = new ReasonabilityResultsNotifier.ReasonabilityResultsNotifierClient(channel);

            Console.Clear();
            ConsoleHelper.WriteLine("Connecting to stream - Press any key to disconnect", ConsoleColor.DarkGreen);

            while (true)
            {
                using var call = client.GetReasonabilityResultsStream(new Empty(), cancellationToken: source.Token);

                try
                {
                    await foreach (var response in call.ResponseStream.ReadAllAsync(source.Token))
                    {
                        if (!notifiedListeningStatus)
                        {
                            ConsoleHelper.WriteTimestampedOutput("Listening for Reasonability check results updates", ConsoleColor.DarkCyan);
                            retryCount = 0;
                            notifiedListeningStatus = true;
                        }

                        if (response.HasResults)
                        {
                            ConsoleHelper.WriteTimestampedOutput($"{response.ReasonabilityResultsInfo}");
                        }
                    }
                }
                catch (RpcException ex)
                {
                    switch (ex)
                    {
                        case { StatusCode: StatusCode.Unavailable }:
                            notifiedListeningStatus = false;
                            retryCount = await AttemptConnectionRetry(retryCount, ex, source);
                            break;
                        case { StatusCode: StatusCode.Cancelled }:
                            ConsoleHelper.WriteTimestampedOutput("Stream disconnected.", ConsoleColor.DarkCyan);
                            ConsoleHelper.WriteLine(new string('-', Console.WindowWidth));
                            return;
                        default:
                            source.Cancel();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
                    source.Cancel();
                    return;
                }
            }
        });

        await WaitingForStream(streamTask, source);
    }

    private static async Task WaitingForStream(Task streamTask, CancellationTokenSource source)
    {
        while (!Console.KeyAvailable && !streamTask.IsCompleted)
        {
            await Task.Delay(250);
        }

        if (!streamTask.IsCompleted)
        {
            Console.ReadKey(true);
            source.Cancel();
            await streamTask;
        }
    }

    private static async Task<int> AttemptConnectionRetry(int retryCount, RpcException ex, CancellationTokenSource source)
    {
        if (++retryCount <= 5)
        {
            ConsoleHelper.WriteTimestampedOutput(
                $"Status Code [{ex.StatusCode}] - Attempting reconnect -> {retryCount} time(s)", ConsoleColor.DarkYellow);
            await Task.Delay(1000 * retryCount);
        }
        else
        {
            ConsoleHelper.WriteTimestampedOutput($"{ex.Status.Detail}", ConsoleColor.Red);
            source.Cancel();
        }

        return retryCount;
    }
}