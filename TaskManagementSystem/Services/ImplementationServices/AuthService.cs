using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTOs.Auth.Requests;
using TaskManagementSystem.DTOs.Auth.Responses;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.JWT;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.IServices;

namespace TaskManagementSystem.Services.ImplementationServices
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDBContext _context;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<AuthService> _logger;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(
            ApplicationDBContext context,
            JwtOptions jwtOptions,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtOptions = jwtOptions;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // validate input
            if (string.IsNullOrWhiteSpace(request.Username))
                throw ApiException.BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw ApiException.BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw ApiException.BadRequest("Password must be at least 6 characters.");

            // check duplicate email
            var emailTaken = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (emailTaken)
                throw ApiException.Conflict($"A user with email '{request.Email}' already exists.");

            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower()
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Registered new user {UserId} ({Email})", user.Id, user.Email);

            return GenerateAuthResponse(user);
        }


        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw ApiException.BadRequest("Email and password are required.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLower());

            if (user == null)
                throw ApiException.BadRequest("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                throw ApiException.BadRequest("Invalid email or password.");

            return GenerateAuthResponse(user);
        }

        private AuthResponse GenerateAuthResponse(User user)
        {
            var expiresAt = DateTime.UtcNow.AddSeconds(_jwtOptions.Lifetime);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = expiresAt
            };
        }
    }
}