namespace NP.Auction.Client.Contracts
{
    using System;
    using System.Collections.Generic;

    public class BlockList
    {
        public string OrderId { get; set; }
        public string AreaCode { get; set; }
        public string Portfolio { get; set; }
        public string AuctionId { get; set; }
        public string Modifier { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime Modified { get; set; }
        public List<Block> Blocks { get; set; }
        public string Comment { get; set; }
    }
}