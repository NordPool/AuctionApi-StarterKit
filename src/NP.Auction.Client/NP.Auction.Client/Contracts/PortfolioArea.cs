namespace NP.Auction.Client.Contracts
{
    public class PortfolioArea
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string EicCode { get; set; }
        public decimal CurveMinVolumeLimit { get; set; }
        public decimal CurveMaxVolumeLimit { get; set; }
    }
}