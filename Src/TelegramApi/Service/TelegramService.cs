using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramApi.Interface;

namespace TelegramApi.Service;

public class TelegramService(IConfiguration configuration) : ITelegramService
{
    private readonly ITelegramBotClient _telegramService =
        new TelegramBotClient(configuration.GetValue<string>("Telegram:ApiKey")!);

    private readonly string _channelId =
        configuration.GetValue<string>("Telegram:ChannelId")!;

    private readonly string _adminName =
        configuration.GetValue<string>("Telegram:AdminName")!;

    public async Task SendMessage(string? coinName, string? price)
    {
        if(coinName is null || price is null) return;
        var message = $"<blockquote>Name: {coinName}</blockquote>\n" +
                      $"<blockquote>Price: {price}</blockquote>\n" +
                      $"<blockquote>Price: {price}</blockquote>";
        await this._telegramService.SendMessage(this._channelId, message, ParseMode.Html);
    }
}