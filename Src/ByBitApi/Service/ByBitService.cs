using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ByBitApi.Interface;
using ByBitApi.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Websocket.Client;

namespace ByBitApi.Service;

public class ByBitService : IByBitService
{
    private WebsocketClient _webSocket;
    public bool IsStarted => _webSocket.IsStarted;
    public event IByBitService.CostHandler? ChangeProgress;
    private ByBitQuery? _lastQuery;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly RestClient _restClient = new("https://api.bybit.com/");

    public ByBitService(IConfiguration configuration)
    {
        this._apiKey = configuration["ByBitApi:ApiKey"]!;
        this._apiSecret = configuration["ByBitApi:ApiSecret"]!;
        this.Create();
    }

    private void Create()
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var url = new Uri("wss://stream.bybit.com/v5/public/spot");
        this._webSocket = new WebsocketClient(url)
        {
            ReconnectTimeout = TimeSpan.FromSeconds(10)
        };
        this._webSocket.ReconnectionHappened.Subscribe(_ =>
        {
            if (_lastQuery != null)
                this._webSocket.Send(JsonSerializer.Serialize(_lastQuery));
        });
        this._webSocket.MessageReceived
            .Where(msg => !string.IsNullOrWhiteSpace(msg.Text))
            .Subscribe(msg =>
            {
                try
                {
                    this.ChangeProgress?.Invoke(JsonSerializer.Deserialize<ByBitResponse>(msg.Text!, options)!);
                }
                catch
                {
                    // ignored
                }
            });
    }

    public async Task CheckIsStarted()
    {
        if (_webSocket.IsRunning) return;
        if (_lastQuery is null) return;

        await this.ReStartAsync(_lastQuery);
    }

    public async Task StartAsync(ByBitQuery query)
    {
        _lastQuery = query;
        await _webSocket.Start();
        this._webSocket.Send(JsonSerializer.Serialize(query));
    }

    public async Task ReStartAsync(ByBitQuery query)
    {
        this.Stop();
        await this.StartAsync(query);
    }

    public async Task<string?> GetBalance()
    {
        var request = this.GetRequest("/v5/account/wallet-balance", queries: [("accountType", "UNIFIED")]);
        var response = await _restClient.ExecuteAsync<ByBitWalletBalance>(request);
        return response.Content;
    }

    private void Stop()
    {
        this._webSocket.Dispose();
        this.Create();
    }

    private RestRequest GetRequest(string url, Method method = Method.Get,
        List<(string Name, object Value)>? queries = null, object? body = null)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var recvWindow = "5000";
        var queryString = queries is null ? null : string.Join("&", queries.Select(x => $"{x.Name}={x.Value}"));
        var signature = Sign(timestamp, _apiKey, recvWindow, queryString, _apiSecret);
        var request = new RestRequest($"{url}?{queryString}", method);
        request.AddHeader("X-BAPI-API-KEY", _apiKey);
        request.AddHeader("X-BAPI-SIGN", signature);
        request.AddHeader("X-BAPI-TIMESTAMP", timestamp);
        request.AddHeader("X-BAPI-RECV-WINDOW", recvWindow);
        if (body != null)
            request.AddJsonBody(body);
        return request;
    }

    private static string Sign(string timestamp, string apiKey, string recvWindow, string? queryString, string secret)
    {
        var payload = timestamp + apiKey + recvWindow + queryString;
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}