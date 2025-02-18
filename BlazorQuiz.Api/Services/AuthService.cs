using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlazorQuiz.Api.Data;
using BlazorQuiz.Api.Data.Entities;
using BlazorQuiz.Shared.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlazorQuiz.Api.Services
{
    public class AuthService
    {
        private readonly QuizContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly IConfiguration _configuration;
        public AuthService(QuizContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration
            )
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.
                AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == dto.Username);
            if (user == null)
            {
                return new AuthResponseDto(default, "Invalid username");
            }
           var password= _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (password == PasswordVerificationResult.Failed)
            {
                return new AuthResponseDto(default, "Invalid password");
            }
            var jwt = GenerateJwtToken(user);
            var loggedInUser = new LoggedInUser(user.Id, user.Name, user.Role,jwt);
            return new AuthResponseDto(loggedInUser);

        }

        private  string GenerateJwtToken(User user)
        {
            Claim[] claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                     
                ];
            var secretKey = _configuration.GetValue<string>("Jwt:Secret");
            var symmetriKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(symmetriKey, SecurityAlgorithms.HmacSha256);
            var jwtSecuritytoken = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
                audience: _configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("Jwt:ExpireInMinutes")),
                signingCredentials: signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecuritytoken);
            return token;
        }
    }
}
