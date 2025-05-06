namespace ByBitApi.Models;

public class ByBitMinMaxSpot(string topic, double priceMin, double priceMax)
{
    public string Topic { get; set; } = topic;
    public double PriceMin { get; set; } = priceMin;
    public double PriceMax { get; set; } = priceMax;
}