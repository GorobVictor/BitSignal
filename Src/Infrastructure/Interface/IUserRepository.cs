using Core.Entity;

namespace Infrastructure.Interface;

public interface IUserRepository
{
    Task<User?> Login(string username, string password);
}