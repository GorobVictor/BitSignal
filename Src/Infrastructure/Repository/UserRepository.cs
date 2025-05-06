using Core.Entity;
using Infrastructure.Context;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class UserRepository(BitSignalContext context) : IUserRepository
{
    public async Task<User?> Login(string username, string password)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
    }
}