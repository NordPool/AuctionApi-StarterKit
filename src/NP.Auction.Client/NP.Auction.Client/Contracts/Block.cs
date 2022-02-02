namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class Block
    {
        public string Modifier { get; set; }
        public OrderStateType State { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal MinimumAcceptanceRatio { get; set; }
        public string LinkedTo { get; set; }
        public string ExclusiveGroup { get; set; }
        public IEnumerable<Period> Periods { get; set; }
        public bool IsSpreadBlock { get; set; }

        [JsonIgnore] public bool IsLinkedBlock => !string.IsNullOrEmpty(LinkedTo) && !IsSpreadBlock;

        [JsonIgnore] public bool IsExclusiveGroup => !string.IsNullOrEmpty(ExclusiveGroup);

        [JsonIgnore] public bool IsProfiledBlock => Periods.Select(x => x.Volume).Distinct().Count() > 1;
    }
}