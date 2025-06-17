using Api.DTOs;
using Api.Interfaces;
using Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace Api.Services;

public class UserService : IUserService
{
    private static readonly List<User> _users = new();

    public async Task<UserResponse> CreateUserAsync(UserCreateRequest request)
    {
        var user = new User
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = HashPassword(request.Senha)
        };

        _users.Add(user);
        return await Task.FromResult(UserResponse.FromEntity(user));
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return await Task.FromResult(user != null ? UserResponse.FromEntity(user) : null);
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        return await Task.FromResult(_users.Select(UserResponse.FromEntity));
    }

    public async Task<bool> UpdateUserAsync(Guid id, UserUpdateRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        user.Nome = request.Nome;
        user.Email = request.Email;
        user.SenhaHash = HashPassword(request.Senha);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        _users.Remove(user);
        return await Task.FromResult(true);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashBytes);
    }
}