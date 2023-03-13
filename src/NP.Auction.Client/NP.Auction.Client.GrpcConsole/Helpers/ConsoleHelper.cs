namespace NP.Auction.Client.GrpcConsole.Helpers;

public static class ConsoleHelper
{
    public static void WriteLine(string output, ConsoleColor? color = null)
    {
        var currentColor = Console.ForegroundColor;

        if (color is not null)
        {
            Console.ForegroundColor = (ConsoleColor)color;
        }

        Console.WriteLine(output);
        Console.ForegroundColor = currentColor;
    }

    public static void WriteTimestampedOutput(string output, ConsoleColor? color = null)
    {
        WriteLine($"[{DateTime.Now.ToLongTimeString()}] {output}", color);
    }
}