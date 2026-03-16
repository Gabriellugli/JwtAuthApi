using JwtAuthApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthApi.Services
{
    public class TokenService
    {
        private static readonly string SecretKey = "minha_chave_super_secreta_para_token_jwt_123456";
        private static Dictionary<string, (string Username, DateTime ExpiresAt)> _refreshTokens = new Dictionary<string, (string, DateTime)>();

        public static AuthResponse GenerateTokens(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            _refreshTokens[refreshToken] = (user.Username, DateTime.UtcNow.AddDays(7));

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")),
                ExpiresAt = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow.AddMinutes(15),
        TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))
            };
        }

        public static AuthResponse? RefreshTokens(string refreshToken)
        {
            if (!_refreshTokens.TryGetValue(refreshToken, out var data))
                return null;

            if (data.ExpiresAt < DateTime.UtcNow)
            {
                _refreshTokens.Remove(refreshToken);
                return null;
            }

            _refreshTokens.Remove(refreshToken);

            var user = new User { Username = data.Username };
            return GenerateTokens(user);
        }

        private static string GenerateAccessToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);

            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}