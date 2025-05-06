using System.Text.Json.Serialization;

namespace ByBitApi.Models;

public class ByBitWalletBalance
{
    public int? RetCode { get; set; }
    public string? RetMsg { get; set; }
    public Result? Result { get; set; }
    public long? Time { get; set; }
}

public class Result
{
    public List[]? List { get; set; }
}

public class List
{
    public string? TotalEquity { get; set; }
    [JsonPropertyName("accountIMRate")] public string? AccountImRate { get; set; }
    public string? TotalMarginBalance { get; set; }
    public string? TotalInitialMargin { get; set; }
    public string? AccountType { get; set; }
    public string? TotalAvailableBalance { get; set; }
    [JsonPropertyName("accountMMRate")] public string? AccountMmRate { get; set; }
    [JsonPropertyName("totalPerpUPL")] public string? TotalPerpUpl { get; set; }
    public string? TotalWalletBalance { get; set; }
    [JsonPropertyName("accountLTV")] public string? AccountLtv { get; set; }
    public string? TotalMaintenanceMargin { get; set; }
    public Coin[]? Coin { get; set; }
}

public class Coin
{
    public string? AvailableToBorrow { get; set; }
    public string? Bonus { get; set; }
    public string? AccruedInterest { get; set; }
    public string? AvailableToWithdraw { get; set; }
    [JsonPropertyName("totalOrderIM")] public string? TotalOrderIm { get; set; }
    public string? Equity { get; set; }
    [JsonPropertyName("totalPositionMM")] public string? TotalPositionMm { get; set; }
    public string? UsdValue { get; set; }
    public string? UnrealisedPnl { get; set; }
    public bool? CollateralSwitch { get; set; }
    public string? SpotHedgingQty { get; set; }
    public string? BorrowAmount { get; set; }
    [JsonPropertyName("totalPositionIM")] public string? TotalPositionIm { get; set; }
    public string? WalletBalance { get; set; }
    public string? CumRealisedPnl { get; set; }
    public string? Locked { get; set; }
    public bool? MarginCollateral { get; set; }
    [JsonPropertyName("coin")] public string? CoinName { get; set; }
}