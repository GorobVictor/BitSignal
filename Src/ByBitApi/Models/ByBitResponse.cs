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
    public string? HighPrice24H { get; set; }
    public string? LowPrice24H { get; set; }
    public string? PrevPrice24H { get; set; }
    public string? Volume24H { get; set; }
    public string? Turnover24H { get; set; }
    public string? Price24HPcnt { get; set; }
    public string? UsdIndexPrice { get; set; }
}