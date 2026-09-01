# AuctionApi-StarterKit
Rapid onboarding starter kit for .NET 10 for the Nord Pool Auctions API

## Content
This solution contains the following projects:

### NP.Auction.Client
- A .NET 10 library containing a client for NP Auction REST API with all contracts and handling of authentication/authorization through a separate SSO API Client
- REST API clients have been constructed utilizing 3rd party library [Refit](https://github.com/reactiveui/refit)

### NP.Auction.Client.Console
- A .NET 10 Console application that demonstrates all key functionalities of the Auction REST API by utilizing the NP.Auction.Client library.
  - Before usage, please specify proper username/password combination in `appsettings.json`

### NP.Auction.Client.GrpcConsole
- A .NET 10 Console application that demonstrates all key functionalities of the Auction gRPC API by utilizing the proto files in NP.Auction.Client library for client generation.
  - Before usage, please specify proper username/password combination in `appsettings.json`

### NP.Auction.Client.Core.Extensions
- A .NET 10 library that contains a small extension method for easier registration of the Auction REST API Client using .NET built-in DI

