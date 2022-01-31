namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class PortfolioVolumesResponse
    {
        public string AuctionId { get; set; }
        public List<PortfolioNetVolume> PortfolioNetVolumes { get; set; }
    }
}