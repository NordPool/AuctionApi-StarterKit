namespace NP.Auction.Client.Core.Extensions.Configuration
{
    using Client.Configuration;

    public class AuctionApiOptions
    {
        public AuthConfig AuthConfig { get; set; }
        public string AuctionApiUrl { get; set; }
    }
}