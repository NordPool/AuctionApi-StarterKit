namespace NP.Auction.Client.Core.Extensions
{
    using System;
    using API;
    using Client.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Refit;
    using Utils;

    public static class ServiceCollectionExtensions
    {
        public static void AddAuctionApiClient(this IServiceCollection services, string auctionApiUrl,
            AuthConfig authConfig)
        {
            services.AddSingleton(authConfig);

            services.AddRefitGeneratedClient<ISsoApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(authConfig.SsoUrl));

            services.AddSingleton<ICachedSsoApiClient, CachedSsoApiClient>();

            services.AddRefitGeneratedClient<IAuctionApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(auctionApiUrl))
                .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();
        }
    }
}