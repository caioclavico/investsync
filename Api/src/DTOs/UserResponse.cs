using Api.Models;

namespace Api.DTOs;

public class UserResponse
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }

    public static UserResponse FromEntity(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Nome = user.Nome,
            Email = user.Email
        };
    }
}
