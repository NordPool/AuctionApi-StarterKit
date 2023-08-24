using System.Collections.Generic;

namespace NP.Auction.Client.Contracts
{
    public class AreaContractGroup
    {
        public string AreaCode { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
    }
}