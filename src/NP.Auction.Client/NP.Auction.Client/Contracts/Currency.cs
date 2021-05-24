namespace NP.Auction.Client.Contracts
{
    public class Currency
    {
        public string CurrencyCode { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
