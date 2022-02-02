using System;
using System.Collections.Generic;

namespace NP.Auction.Client.Contracts
{
    public class PricesResponse
    {
        public string Auction { get; set; }
        public DateTime AuctionDeliveryStart { get; set; }
        public DateTime AuctionDeliveryEnd { get; set; }
        public List<ContractPrice> Contracts { get; set; }
    }
}