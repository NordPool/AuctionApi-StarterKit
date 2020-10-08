namespace NP.Auction.Client.Contracts
{
    using System;
    using System.Collections.Generic;

    public class Auction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AuctionStateType State { get; set; }
        public DateTime CloseForBidding { get; set; }
        public DateTime DeliveryStart { get; set; }
        public DateTime DeliveryEnd { get; set; }
        public List<Contract> Contracts { get; set; }
    }
}