namespace NP.Auction.Client.Contracts
{
    using System;

    public class Contract
    {
        public string Id { get; set; }
        public DateTime DeliveryStart { get; set; }
        public DateTime DeliveryEnd { get; set; }
    }
}