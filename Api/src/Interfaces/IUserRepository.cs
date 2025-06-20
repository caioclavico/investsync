using InvestSync.Api.src.Models;

namespace InvestSync.Api.src.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
    }
}