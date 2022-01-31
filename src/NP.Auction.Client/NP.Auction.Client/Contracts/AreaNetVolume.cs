namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class AreaNetVolume
    {
        public string AreaCode { get; set; }
        public List<ContractNetVolume> NetVolumes { get; set; }
    }
}