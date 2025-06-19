using Microsoft.AspNetCore.Mvc;
using InvestSync.Api.src.DTOs;
using InvestSync.Api.src.Models;
using InvestSync.Api.src.Interfaces;

namespace InvestSync.Api.src.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepositories _userRepository;

        public AuthController(IUserRepositories userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(UserRegisterRequest request)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email);
            if (existing != null)
                return Conflict("E-mail já cadastrado.");

            // Simples hash para exemplo (não use em produção!)
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Password))
            };

            var created = await _userRepository.CreateAsync(user);

            return Ok(new UserResponse
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login(UserLoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Usuário ou senha inválidos.");

            var passwordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Password));
            if (user.PasswordHash != passwordHash)
                return Unauthorized("Usuário ou senha inválidos.");

            return Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }
    }
}