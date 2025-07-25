namespace NP.Auction.Client.Contracts
{
    using System;
    using System.Collections.Generic;

    public class CancelAllOrdersResponse
    {
        public List<Guid> CancelledOrderIds { get; set; }
    }
}