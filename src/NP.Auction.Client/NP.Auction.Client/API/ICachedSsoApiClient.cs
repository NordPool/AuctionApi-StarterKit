namespace NP.Auction.Client.API
{
    using System.Threading.Tasks;

    public interface ICachedSsoApiClient
    {
        Task<string> GetAuthTokenAsync();
    }
}