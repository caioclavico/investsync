using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class UserUpdateRequest
{
    [Required]
    public required string Nome { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, MinLength(6)]
    public required string Senha { get; set; }
}
