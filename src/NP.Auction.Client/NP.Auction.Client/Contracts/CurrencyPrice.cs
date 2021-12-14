namespace NP.Auction.Client.Contracts
{
    public class CurrencyPrice
    {
        public string CurrencyCode { get; set; }
        public decimal? MarketPrice { get; set; }
        public AuctionResultState Status { get; set; }
    }
}