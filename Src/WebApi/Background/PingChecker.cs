using RestSharp;

namespace WebApi.Background;

public class PingChecker(IConfiguration configuration) : BackgroundService
{
    private readonly RestClient _client = new(configuration.GetValue<string>("Domain")!);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1);

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                _ = await _client.ExecuteGetAsync("/Api/ByBit/Ping", cancellationToken: stoppingToken);
            }
            catch
            {
                // ignored
            }
            finally
            {
                await Task.Delay(10000);
            }
    }
}