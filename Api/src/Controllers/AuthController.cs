using Microsoft.AspNetCore.Mvc;
using InvestSync.Api.src.DTOs;
using InvestSync.Api.src.Models;
using InvestSync.Api.src.Interfaces;
using InvestSync.Api.src.Helpers;
using System.Text;

namespace InvestSync.Api.src.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "crie-uma-chave-secreta-forte-no-arquivo-env";
            _jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "InvestSync";
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(UserRegisterRequest request)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email);
            if (existing != null)
                return Conflict("E-mail já cadastrado.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password))
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
        public async Task<ActionResult<UserLoginResponse>> Login(UserLoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Usuário ou senha inválidos.");

            var passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));
            if (user.PasswordHash != passwordHash)
                return Unauthorized("Usuário ou senha inválidos.");

            var token = JwtHelper.GenerateJwtToken(user, _jwtKey, _jwtIssuer);

            return Ok(new UserLoginResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                }
            });
        }
    }
}