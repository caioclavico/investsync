using System.Collections.Concurrent;
using InvestSync.Api.src.Models;
using InvestSync.Api.src.Interfaces;

namespace InvestSync.Api.src.Repositories
{
    public class InMemoryUserRepository : IUserRepositories
    {
        private static readonly ConcurrentDictionary<string, User> _users = new();

        public Task<User?> GetByEmailAsync(string email)
        {
            _users.TryGetValue(email.ToLower(), out var user);
            return Task.FromResult(user);
        }

        public Task<User> CreateAsync(User user)
        {
            _users[user.Email.ToLower()] = user;
            return Task.FromResult(user);
        }
    }
}