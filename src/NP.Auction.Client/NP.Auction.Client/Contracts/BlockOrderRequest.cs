namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class BlockOrderRequest
    {
        public string AuctionId { get; set; }
        public string Portfolio { get; set; }
        public string AreaCode { get; set; }
        public string Comment { get; set; }
        public List<Block> Blocks { get; set; }
    }
}