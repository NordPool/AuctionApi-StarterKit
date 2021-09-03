namespace NP.Auction.Client.Console.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;

    public static class OrderGenerator
    {
        public static CurveOrderRequest GenerateStaticCurveOrderRequest(string portfolio, string areaCode,
            Auction auction, double minPrice = -500.0d, double maxPrice = -1500.0d)
        {
            var curveOrderRequest = new CurveOrderRequest
            {
                AreaCode = areaCode,
                Portfolio = portfolio,
                AuctionId = auction.Id,
                Comment = $"CurveOrder_{areaCode}_{portfolio}",
                Curves = GenerateCurves(auction, minPrice, maxPrice).ToList()
            };

            return curveOrderRequest;
        }

        public static BlockOrderRequest GenerateStaticBlockOrder(string portfolio, string areaCode,
            Auction auction, BlockOrderType blockOrderType = BlockOrderType.Regular)
        {
            return new BlockOrderRequest
            {
                AreaCode = areaCode,
                AuctionId = auction.Id,
                Portfolio = portfolio,
                Comment = $"BlockOrder_{areaCode}_{portfolio}",
                Blocks = GenerateBlocks(blockOrderType, auction).ToList()
            };
        }

        public static IEnumerable<Block> GenerateBlocks(BlockOrderType blockOrderType, Auction auction)
        {
            var blocks = new List<Block>
            {
                new Block
                {
                    MinimumAcceptanceRatio = 1,
                    Name = blockOrderType == BlockOrderType.Spread ? "SpreadBlock1" : "RegularBlock",
                    Price = 50,
                    IsSpreadBlock = blockOrderType == BlockOrderType.Spread ? true : false,
                    Periods = new List<Period>
                    {
                        new Period
                        {
                            ContractId = auction.Contracts[0].Id,
                            Volume = 200
                        },
                        new Period
                        {
                            ContractId = auction.Contracts[1].Id,
                            Volume = 200
                        }
                    }
                }
            };

            if (blockOrderType == BlockOrderType.Profiled)
                // Profiled with different volumes
                blocks.First().Periods.Last().Volume = 150;

            if (blockOrderType == BlockOrderType.ExclusiveGroup)
            {
                // Exclusive group should have same value on ExclusiveGroup field
                var exclusiveGroupName = "ExclGroup_1";
                blocks.First().ExclusiveGroup = exclusiveGroupName;
                blocks.Add(new Block
                {
                    MinimumAcceptanceRatio = 1,
                    ExclusiveGroup = exclusiveGroupName,
                    Name = "RegularBlock2",
                    Price = 50,
                    Periods = new List<Period>
                    {
                        new Period
                        {
                            ContractId = auction.Contracts[0].Id,
                            Volume = 150
                        },
                        new Period
                        {
                            ContractId = auction.Contracts[1].Id,
                            Volume = 150
                        }
                    }
                });
            }

            if (blockOrderType == BlockOrderType.Linked)
                // Linked block should have LinkedTo field pointing to parent block name
                blocks.Add(new Block
                {
                    MinimumAcceptanceRatio = 1,
                    LinkedTo = blocks.First().Name,
                    Name = "LinkedBlock1",
                    Price = 50,
                    Periods = new List<Period>
                    {
                        new Period
                        {
                            ContractId = auction.Contracts[0].Id,
                            Volume = 150
                        },
                        new Period
                        {
                            ContractId = auction.Contracts[1].Id,
                            Volume = 150
                        }
                    }
                });

            if (blockOrderType == BlockOrderType.Spread)
            {
                var firstSpread = blocks.First();
                // Spread blocks should always contain a buy and sell block
                blocks.Add(new Block
                {
                    MinimumAcceptanceRatio = 1,
                    LinkedTo = firstSpread.Name,
                    Name = "SpreadBlock2",
                    Price = 50,
                    IsSpreadBlock = true,
                    Periods = new List<Period>
                    {
                        new Period
                        {
                            ContractId = auction.Contracts[2].Id,
                            Volume = -150
                        },
                        new Period
                        {
                            ContractId = auction.Contracts[3].Id,
                            Volume = -150
                        }
                    }
                });

                var lastSpread = blocks.Last();
                firstSpread.LinkedTo = lastSpread.Name;
            }

            return blocks;
        }

        public static IEnumerable<Curve> GenerateCurves(Auction auction, double minPrice, double maxPrice)
        {
            var curves = new List<Curve>();
            foreach (var contract in auction.Contracts)
            {
                var curvePoints = GenerateCurvePoints(minPrice, maxPrice);
                curves.Add(new Curve
                {
                    ContractId = contract.Id,
                    CurvePoints = curvePoints.ToList()
                });
            }

            return curves;
        }

        private static IEnumerable<CurvePoint> GenerateCurvePoints(double minPrice, double maxPrice)
        {
            var curvePoints = new List<CurvePoint>();
            var startingVolume = 500;
            var numberOfPriceSteps = 20;
            var totalPriceDifference = maxPrice - minPrice;
            var priceStep = totalPriceDifference / numberOfPriceSteps;
            var volumeStep = 50;

            // Add min price point
            curvePoints.Add(new CurvePoint
            {
                Price = minPrice,
                Volume = startingVolume
            });

            for (var i = 1; i <= numberOfPriceSteps; i++)
            {
                var curvePoint = new CurvePoint
                {
                    Price = minPrice + i * priceStep,
                    Volume = startingVolume - (i - 1) * volumeStep
                };
                curvePoints.Add(curvePoint);
            }

            return curvePoints;
        }
    }


    public enum BlockOrderType
    {
        Regular,
        ExclusiveGroup,
        Linked,
        Profiled,
        Spread
    }
}