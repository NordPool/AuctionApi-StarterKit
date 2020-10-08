namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class OrdersResponse
    {
        public List<CurveOrder> CurveOrders { get; set; }
        public List<BlockList> BlockLists { get; set; }
    }
}