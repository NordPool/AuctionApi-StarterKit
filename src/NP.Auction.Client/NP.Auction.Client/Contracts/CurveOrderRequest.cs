namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class CurveOrderRequest
    {
        public string AuctionId { get; set; }
        public string Portfolio { get; set; }
        public string AreaCode { get; set; }
        public string Comment { get; set; }
        public List<Curve> Curves { get; set; }
    }
}