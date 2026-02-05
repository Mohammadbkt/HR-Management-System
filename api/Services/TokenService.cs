using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.api.Services
{
    public interface ITokenService
    {
        (string token, DateTime expires) GenerateToken(int userId, string username, List<string> roles);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string token, DateTime expires) GenerateToken(int userId, string username, List<string> roles)
        {

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));


            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = _configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Key not configured in appsettings.json");
            
             var issuer = _configuration["Jwt:Issuer"] ?? "HRMS.api";
            var audience = _configuration["Jwt:Audience"] ?? "HRMS.client";
            
            // Get expiration - try multiple possible configuration keys
            var expiresInMinutes = GetExpirationMinutes();
            var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);


        }

         private double GetExpirationMinutes()
        {
            // Try different configuration key names
            var config = _configuration;
            
            if (double.TryParse(config["Jwt:ExpiresInMinutes"], out double minutes))
                return minutes;
            
            if (double.TryParse(config["Jwt:AccessTokenExpirationMinutes"], out minutes))
                return minutes;
            
            if (double.TryParse(config["Jwt:ExpirationMinutes"], out minutes))
                return minutes;
            
            if (double.TryParse(config["Jwt:Expires"], out minutes))
                return minutes;
            
            // Default: 60 minutes
            return 60;
        }

    }
}