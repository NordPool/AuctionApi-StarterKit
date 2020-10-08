namespace NP.Auction.Client.API
{
    using System.Threading.Tasks;
    using Contracts.SSO;
    using Refit;

    public interface ISsoApiClient
    {
        [Post("/connect/token")]
        Task<TokenResponse> PostTokenAsync([Header("Authorization")] string basicAuth,
            [Body(BodySerializationMethod.UrlEncoded)]
            TokenRequest tokenRequest);
    }
}