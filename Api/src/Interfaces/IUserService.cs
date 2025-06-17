using Api.DTOs;

namespace Api.Interfaces;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(UserCreateRequest request);
    Task<UserResponse?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(Guid id, UserUpdateRequest request);
    Task<bool> DeleteUserAsync(Guid id);
}
