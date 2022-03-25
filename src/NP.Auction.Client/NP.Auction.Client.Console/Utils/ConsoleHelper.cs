namespace NP.Auction.Client.Console.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;

    public static class ConsoleHelper
    {
        public static void WriteAuctionsInfo(IEnumerable<Auction> auctions)
        {
            Console.WriteLine("Open auctions:");

            foreach (var auction in auctions.Where(x => x.State == AuctionStateType.Open))
                Console.WriteLine(auction.Id);

            Console.WriteLine();

            Console.WriteLine("Closed auctions:");

            foreach (var auction in auctions.Where(x => x.State == AuctionStateType.Closed))
                Console.WriteLine(auction.Id);

            Console.WriteLine();


            Console.WriteLine("ResultsPublished auctions:");

            foreach (var auction in auctions.Where(x => x.State == AuctionStateType.ResultsPublished))
                Console.WriteLine(auction.Id);
        }

        public static void WriteDetailedAuctionInfo(Auction auction)
        {
            Console.WriteLine($"Selected auction: {auction.Id}");
            Console.WriteLine($"Supported currencies with Min and Max prices are following:");

            foreach (var currencyInfo in auction.Currencies)
            {
                WriteCurrencyInfo(currencyInfo);
            }

            Console.WriteLine($"Available portfolios for selected auction are following:");

            foreach (var portfolio in auction.Portfolios)
            {
                WritePortfolioInfo(portfolio);
            }
        }

        private static void WriteCurrencyInfo(Currency currency)
        {
            Console.WriteLine($"\t{currency.CurrencyCode}");
            Console.WriteLine($"\tMin: {currency.MinPrice} - Max: {currency.MaxPrice}");
            Console.WriteLine("---");
        }

        private static void WritePortfolioInfo(Portfolio portfolio)
        {
            Console.WriteLine($"\tId: {portfolio.Id}");
            Console.WriteLine($"\tName: {portfolio.Name}");
            Console.WriteLine($"\tCompany: {portfolio.CompanyName}");
            Console.WriteLine($"\tCurrency: {portfolio.Currency}");
            Console.WriteLine($"\tPermission: {portfolio.Permission}");
            Console.WriteLine("\tTradable areas for portfolio (area code - name - eic code):");
            foreach (var area in portfolio.Areas)
            {
                Console.WriteLine($"\t\t{area.Code} - {area.Name} - {area.EicCode}");
            }
            Console.WriteLine("---");
        }

        public static void WriteOrdersInfo(OrdersResponse orders)
        {
            Console.WriteLine($"Listing {orders.CurveOrders.Count} curve orders:");
            foreach (var curveOrder in orders.CurveOrders) WriteCurveOrder(curveOrder);
            Console.WriteLine();
            Console.WriteLine($"Listing {orders.BlockLists.Count} block orders");
            foreach (var blockList in orders.BlockLists) WriteBlockList(blockList);
        }

        public static void WriteBlockOrderRequest(BlockOrderRequest blockOrderRequest)
        {
            Console.WriteLine($"AuctionId: {blockOrderRequest.AuctionId}");
            Console.WriteLine($"AreaCode: {blockOrderRequest.AreaCode}");
            Console.WriteLine($"Portfolio: {blockOrderRequest.Portfolio}");
            Console.WriteLine($"Comment: {blockOrderRequest.Comment}");

            Console.WriteLine("Blocks:");

            foreach (var block in blockOrderRequest.Blocks) WriteBlock(block);
        }

        public static void WriteCurveOrder(CurveOrder curveOrder)
        {
            Console.WriteLine("---");
            Console.WriteLine($"OrderId: {curveOrder.OrderId}");
            Console.WriteLine($"Area: {curveOrder.AreaCode}");
            Console.WriteLine($"State: {curveOrder.State}");
            Console.WriteLine($"Portfolio: {curveOrder.Portfolio}");
            Console.WriteLine($"Company: {curveOrder.CompanyName}");
            Console.WriteLine($"Comment: {curveOrder.Comment}");
            WriteCurves(curveOrder);
            Console.WriteLine("---");
        }

        public static void WriteCurveOrderRequest(CurveOrderRequest curveOrderRequest)
        {
            Console.WriteLine("---");
            Console.WriteLine($"AuctionId: {curveOrderRequest.AuctionId}");
            Console.WriteLine($"AreaCode: {curveOrderRequest.AreaCode}");
            Console.WriteLine($"Portfolio: {curveOrderRequest.Portfolio}");
            Console.WriteLine($"Comment: {curveOrderRequest.Comment}");
            WriteCurves(curveOrderRequest);
            Console.WriteLine("---");
        }

        private static void WriteCurves(CurveOrder curveOrder)
        {
            Console.WriteLine("Curves:");
            foreach (var curve in curveOrder.Curves)
            {
                Console.WriteLine($"Curve contract: {curve.ContractId}");
                Console.WriteLine($"Prices:\t\t{string.Join("\t", curve.CurvePoints.Select(x => x.Price))}");
                Console.WriteLine($"Volumes:\t{string.Join("\t", curve.CurvePoints.Select(x => x.Volume))}");
                Console.WriteLine("");
            }
        }

        private static void WriteCurves(CurveOrderRequest curveOrderRequest)
        {
            Console.WriteLine("Curves:");
            foreach (var curve in curveOrderRequest.Curves)
            {
                Console.WriteLine($"Curve contract: {curve.ContractId}");
                Console.WriteLine($"Prices:\t\t{string.Join("\t", curve.CurvePoints.Select(x => x.Price))}");
                Console.WriteLine($"Volumes:\t{string.Join("\t", curve.CurvePoints.Select(x => x.Volume))}");
                Console.WriteLine("");
            }
        }

        public static CommandType RequestSelectedAuctionCommand(Auction auction)
        {
            if (auction.State == AuctionStateType.Open)
            {
                Console.WriteLine(
                    "Available options: \"Orders\", \"PlaceCurve\", \"PlaceBlocks\", \"ModifyCurve\", \"ModifyBlock\", \"CancelCurve\",  \"CancelBlock\", \"Auctions\", \"Exit\". Specify one of the options:");
                CommandType command;
                while (!Enum.TryParse(Console.ReadLine(), out command))
                    Console.WriteLine("Incorrect option specified! Try again.");

                return command;
            }

            if (auction.State == AuctionStateType.ResultsPublished)
            {
                Console.WriteLine(
                    "Available options: \"Orders\", \"Trades\", \"Prices\", \"PortfolioVolumes\", \"Auctions\", \"Exit\". Specify one of the options:");
                CommandType command;
                while (!Enum.TryParse(Console.ReadLine(), out command))
                    Console.WriteLine("Incorrect option specified! Try again.");

                return command;
            }

            if (auction.State == AuctionStateType.Closed)
            {
                Console.WriteLine("Available options: \"Orders\", \"Auctions\", \"Exit\". Specify one of the options:");
                CommandType command;
                while (!Enum.TryParse(Console.ReadLine(), out command))
                    Console.WriteLine("Incorrect option specified! Try again.");

                return command;
            }

            return CommandType.None;
        }

        public static void WriteTradesInfo(IEnumerable<TradesSummary> trades)
        {
            Console.WriteLine("Trades fetched:");
            Console.WriteLine("Trades for Curve Orders");
            foreach (var tradesSummary in trades.Where(x => x.OrderType == OrderType.Curve)) WriteTradeSummary(tradesSummary, OrderType.Curve);

            Console.WriteLine("Trades for Block Orders");
            foreach (var tradesSummary in trades.Where(x => x.OrderType == OrderType.Block)) WriteTradeSummary(tradesSummary, OrderType.Block);
        }

        private static void WriteTradeSummary(TradesSummary tradesSummary, OrderType orderType)
        {
            Console.WriteLine($"AuctionId: {tradesSummary.AuctionId}");
            Console.WriteLine($"AreaCode: {tradesSummary.AreaCode}");
            Console.WriteLine($"Company: {tradesSummary.CompanyName}");
            Console.WriteLine($"Portfolio: {tradesSummary.Portfolio}");
            Console.WriteLine($"Currency: {tradesSummary.CurrencyCode}");
            if (orderType == OrderType.Block)
            {
                Console.WriteLine($"Block name: {tradesSummary.Name}");
                Console.WriteLine($"ExclusiveGroup: {tradesSummary.ExclusiveGroup}");
                Console.WriteLine($"LinkedTo: {tradesSummary.LinkedTo}");
                Console.WriteLine($"IsSpreadBlock: {tradesSummary.IsSpreadBlock}");
            }

            WriteTrades(tradesSummary.Trades);
        }

        private static void WriteTrades(IEnumerable<Trade> trades)
        {
            foreach (var trade in trades)
            {
                Console.WriteLine($"TradeId: {trade.TradeId}");
                Console.WriteLine($"Contract: {trade.ContractId}");
                Console.WriteLine($"Price: {trade.Price}");
                Console.WriteLine($"Volumes: {trade.Volume}");
                Console.WriteLine($"TradeSide: {trade.Side}");
                Console.WriteLine($"Status: {trade.Status}");
                Console.WriteLine("");
            }
        }

        public static void WritePricesInfo(PricesResponse prices)
        {
            Console.WriteLine($"\nAuctionId: {prices.Auction}");
            Console.WriteLine();

            foreach (var contract in prices.Contracts) WritePrice(contract);
        }

        private static void WritePrice(ContractPrice contract)
        {
            Console.WriteLine($"ContractId: {contract.ContractId}");
            Console.WriteLine($"DeliveryStart: {contract.DeliveryStart}");
            Console.WriteLine($"DeliveryEnd: {contract.DeliveryEnd}");

            foreach (var area in contract.Areas)
            {
                Console.WriteLine($"\tAreaCode: {area.AreaCode}");
                foreach (var price in area.Prices)
                {
                    if (price.MarketPrice.HasValue)
                    {
                        Console.WriteLine($"\tCurrencyCode: {price.CurrencyCode}");
                        Console.WriteLine($"\tMarketPrice: {price.MarketPrice}");
                        Console.WriteLine($"\tStatus: {price.Status}");
                    }
                    else
                    {
                        Console.WriteLine("\tNo price for area");
                    }
                }
                Console.WriteLine();
            }
        }

        public static void WritePortfolioVolumesInfo(PortfolioVolumesResponse portfolioVolumes)
        {
            Console.WriteLine($"\nAuctionId: {portfolioVolumes.AuctionId}");
            Console.WriteLine();

            foreach (var contract in portfolioVolumes.PortfolioNetVolumes) WritePortfolioVolume(contract);
        }

        private static void WritePortfolioVolume(PortfolioNetVolume portfolioVolume)
        {
            Console.WriteLine($"Portfolio: {portfolioVolume.Portfolio}");
            Console.WriteLine($"CompanyName: {portfolioVolume.CompanyName}");

            foreach (var area in portfolioVolume.AreaNetVolumes)
            {
                Console.WriteLine($"\tAreaCode: {area.AreaCode}");
                foreach (var netVolume in area.NetVolumes)
                {
                    var volume = netVolume.NetVolume?.ToString() ?? "No volume available"; 

                    Console.WriteLine($"\tContractId: {netVolume.ContractId}");
                    Console.WriteLine($"\tNetVolume: {volume}");
                    Console.WriteLine($"\tDeliveryStart: {netVolume.DeliveryStart}");
                    Console.WriteLine($"\tDeliveryEnd: {netVolume.DeliveryEnd}");
                }
                Console.WriteLine($"\t---");
            }
        }

        public static void WriteBlockList(BlockList blockList)
        {
            Console.WriteLine($"OrderId: {blockList.OrderId}");
            Console.WriteLine($"AuctionId: {blockList.AuctionId}");
            Console.WriteLine($"AreaCode: {blockList.AreaCode}");
            Console.WriteLine($"Portfolio: {blockList.Portfolio}");
            Console.WriteLine($"Company: {blockList.CompanyName}");
            Console.WriteLine($"Comment: {blockList.Comment}");
            Console.WriteLine("Blocks:");

            foreach (var block in blockList.Blocks) WriteBlock(block);
        }

        private static void WriteBlock(Block block)
        {
            Console.WriteLine($"\tName: {block.Name}");
            Console.WriteLine($"\tPrice: {block.Price}");
            Console.WriteLine($"\tExclusive Group: {block.ExclusiveGroup}");
            Console.WriteLine($"\tLinkedTo: {block.LinkedTo}");
            Console.WriteLine($"\tState: {block.State}");
            Console.WriteLine($"\tIsSpreadBlock: {block.IsSpreadBlock}");
            Console.WriteLine("\tPeriods:");
            Console.WriteLine();
            foreach (var period in block.Periods)
            {
                Console.WriteLine($"\t\tContract: {period.ContractId}");
                Console.WriteLine($"\t\tVolume: {period.Volume}");
            }
        }
    }

    public enum CommandType
    {
        None,
        Orders,
        Trades,
        Prices,
        PortfolioVolumes,
        PlaceCurve,
        PlaceBlocks,
        ModifyCurve,
        ModifyBlock,
        CancelCurve,
        CancelBlock,
        Auctions,
        Exit
    }
}