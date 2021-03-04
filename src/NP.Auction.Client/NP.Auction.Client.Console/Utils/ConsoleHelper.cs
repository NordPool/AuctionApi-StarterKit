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
            WriteCurves(curveOrder);
            Console.WriteLine("---");
        }

        public static void WriteCurveOrderRequest(CurveOrderRequest curveOrderRequest)
        {
            Console.WriteLine("---");
            Console.WriteLine($"AuctionId: {curveOrderRequest.AuctionId}");
            Console.WriteLine($"AreaCode: {curveOrderRequest.AreaCode}");
            Console.WriteLine($"Portfolio: {curveOrderRequest.Portfolio}");
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
                    "Available options: \"Orders\", \"Trades\", \"Auctions\", \"Exit\". Specify one of the options:");
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
            foreach (var tradesSummary in trades.Where(x => x.OrderType == OrderType.Curve)) WriteTrade(tradesSummary);

            Console.WriteLine("Trades for Block Orders");
            foreach (var tradesSummary in trades.Where(x => x.OrderType == OrderType.Block)) WriteTrade(tradesSummary);
        }

        private static void WriteTrade(TradesSummary tradesSummary)
        {
            Console.WriteLine($"AuctionId: {tradesSummary.AuctionId}");
            Console.WriteLine($"AreaCode: {tradesSummary.AreaCode}");
            Console.WriteLine($"Portfolio: {tradesSummary.Portfolio}");
            Console.WriteLine($"Currency: {tradesSummary.CurrencyCode}");
            foreach (var trade in tradesSummary.Trades)
            {
                Console.WriteLine($"TradeId: {trade.TradeId}");
                Console.WriteLine($"Contract: {trade.ContractId}");
                Console.WriteLine($"Price: {trade.Price}");
                Console.WriteLine($"Volumes: {trade.Volume}");
                Console.WriteLine($"TradeSide: {trade.Side}");
                Console.WriteLine("");
            }
        }

        public static void WriteBlockList(BlockList blockList)
        {
            Console.WriteLine($"OrderId: {blockList.OrderId}");
            Console.WriteLine($"AuctionId: {blockList.AuctionId}");
            Console.WriteLine($"AreaCode: {blockList.AreaCode}");
            Console.WriteLine($"Portfolio: {blockList.Portfolio}");
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