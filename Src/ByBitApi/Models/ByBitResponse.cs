using System.Text.Json.Serialization;

namespace ByBitApi.Models;

public class ByBitResponse
{
    public string? Topic { get; set; }
    public long? Ts { get; set; }
    public string? Type { get; set; }
    public long? Cs { get; set; }
    public Data? Data { get; set; }
}

public class Data
{
    public string? Symbol { get; set; }
    public string? LastPrice { get; set; }
    [JsonPropertyName("highPrice24h")] public string? HighPrice24H { get; set; }
    [JsonPropertyName("lowPrice24h")] public string? LowPrice24H { get; set; }
    [JsonPropertyName("prevPrice24h")] public string? PrevPrice24H { get; set; }
    [JsonPropertyName("volume24h")] public string? Volume24H { get; set; }
    [JsonPropertyName("turnover24h")] public string? Turnover24H { get; set; }
    [JsonPropertyName("price24hPcnt")] public string? Price24HPcnt { get; set; }
    public string? UsdIndexPrice { get; set; }
}