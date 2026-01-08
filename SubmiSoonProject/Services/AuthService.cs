using Microsoft.IdentityModel.Tokens;
using SubmiSoonProject.DTOs.Auth;
using SubmiSoonProject.DTOs.Common;
using SubmiSoonProject.Exceptions;
using SubmiSoonProject.Helpers;
using SubmiSoonProject.Models;
using SubmiSoonProject.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SubmiSoonProject.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateUser(LoginRequest request);
    }
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<AuthResponse> AuthenticateUser(LoginRequest request)
        {
            var user = await _userRepository.GetUserDetail(request.Email);
            if (user == null)
            {
                throw new NotFoundException("Invalid credentials");
            }

            if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            var accessToken = GenerateAccessToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "30")
            );

            return new AuthResponse
            {
                User = new UserInfo
                {
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString().ToLower()
                },
                ExpiresAt = expiresAt.ToString("o"),
                AccessToken = accessToken,
                Success = true
            };
        }
        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "30")
                ),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
