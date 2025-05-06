namespace Infrastructure.Interface;

public interface ICacheRepository
{
    Task SetCoinNotification(string coin);

    Task<bool> AnyCoinNotification(string coin);
}