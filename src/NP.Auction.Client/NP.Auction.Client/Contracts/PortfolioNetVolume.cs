namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class PortfolioNetVolume
    {
        public string Portfolio { get; set; }
        public string CompanyName { get; set; }
        public List<AreaNetVolume> AreaNetVolumes { get; set; }
    }
}