namespace NP.Auction.Client.GrpcConsole.Helpers;

using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using NP.Auction.Client.API;
using NP.Auction.Client.Configuration;
using Refit;

public static class ConfigurationHelper
{
    public static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public static ICachedSsoApiClient InitializeSsoClient(IConfiguration config)
    {
        var authConfig = config.GetRequiredSection("SsoAuthentication").Get<AuthConfig>();
        var ssoClient = RestService.For<ISsoApiClient>(authConfig.SsoUrl);

        return new CachedSsoApiClient(authConfig, ssoClient);
    }

    public static GrpcChannelOptions InitializeChannelOptions(IConfiguration config, ICachedSsoApiClient ssoApiClient)
    {
        // Fetch a new token for the request if the current one has expired.
        var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
        {
            var token = await ssoApiClient.GetAuthTokenAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(token))
            {
                metadata.Add("Authorization", $"Bearer {token}");
            }
        });

        return new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Create(ChannelCredentials.SecureSsl, credentials)
        };
    }
}

