namespace NP.Auction.Client.Contracts
{
    using System;

    public class ContractNetVolume
    {
        public decimal? NetVolume { get; set; }
        public string ContractId { get; set; }
        public DateTime DeliveryStart { get; set; }
        public DateTime DeliveryEnd { get; set; }
    }
}