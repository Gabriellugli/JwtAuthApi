using Microsoft.AspNetCore.Mvc;
using JwtAuthApi.Models;
using JwtAuthApi.Services;

namespace JwtAuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            var existingUser = _userService.GetByUsernameAndPassword(user.Username, user.Password);

            if (existingUser != null)
            {
                var tokens = TokenService.GenerateTokens(existingUser);
                return Ok(tokens);
            }

            return Unauthorized("Usuário ou senha inválidos");
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            var tokens = TokenService.RefreshTokens(request.RefreshToken);

            if (tokens == null)
                return Unauthorized("Refresh token inválido ou expirado");

            return Ok(tokens);
        }
    }
}