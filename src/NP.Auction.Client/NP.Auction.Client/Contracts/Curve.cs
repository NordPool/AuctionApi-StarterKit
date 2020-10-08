namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class Curve
    {
        public string ContractId { get; set; }
        public List<CurvePoint> CurvePoints { get; set; }
    }
}