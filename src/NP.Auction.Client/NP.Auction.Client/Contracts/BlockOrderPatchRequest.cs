namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class BlockOrderPatchRequest
    {
        public List<Block> Blocks { get; set; }
        public string Comment { get; set; }
    }
}