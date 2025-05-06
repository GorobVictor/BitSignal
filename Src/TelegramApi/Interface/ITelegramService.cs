namespace TelegramApi.Interface;

public interface ITelegramService
{
    Task SendMessage(string? coinName, string? price);
}