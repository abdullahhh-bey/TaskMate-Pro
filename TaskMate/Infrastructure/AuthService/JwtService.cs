using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskMate.Core.DTO.AuthDTO;
using TaskMate.Core.Models;
using TaskMate.Infrastructure.Data;

namespace TaskMate.Infrastructure.AuthService
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<JwtService> _logger;
        private readonly TaskProDbContext _context;

        public JwtService(IConfiguration config, UserManager<User> userManager, ILogger<JwtService> logger, TaskProDbContext context)
        {
            _config = config;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<string> GenerateToken(User user)
        {
            _logger.LogInformation("Generating Token {Time}:", DateTime.UtcNow);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName ?? "")
            };

            // Fetch roles using UserManager
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            if(roles.Contains("Employee"))
            {
                claims.Add(new Claim("permission", "can-update"));
                claims.Add(new Claim("permission", "can-comment"));
            }

            if (roles.Contains("Manager"))
            {
                claims.Add(new Claim("permission", "can-assign"));
                claims.Add(new Claim("permission", "can-update"));
                claims.Add(new Claim("permission", "can-comment"));
            }

            if (roles.Contains("Admin"))
            {
                claims.Add(new Claim("permission", "can-assign"));
                claims.Add(new Claim("permission", "can-delete"));
                claims.Add(new Claim("permission", "can-update"));
                claims.Add(new Claim("permission", "can-comment"));
            }

            var key =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            _logger.LogInformation("Generated Token {Time}:", DateTime.UtcNow);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        //public async Task<ResponseTokensDTO> ValidateGenerateTokens( RefreshTokenRequestDTO dto)
        //{
            
        //}


        public async Task<RefreshToken> GenerateRefreshToken(User user)
        {
            _logger.LogInformation("Generating Refres Token for {User Email} at {Time}", user.Email, DateTime.UtcNow.ToString());
            
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("A"),
                IsChanged = false,
                Expiry = DateTime.UtcNow.AddHours(1),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
           
        

    }
}
