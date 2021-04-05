using Microsoft.IdentityModel.Tokens;
using ReaderBackend.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReaderBackend.Jwt
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly JwtTokenConfig _jwtTokenConfig;

        public JwtGenerator(JwtTokenConfig config)
        {
            _jwtTokenConfig = config;
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Login)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtTokenConfig.Secret)),
                SecurityAlgorithms.HmacSha512Signature         
                );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtTokenConfig.Issuer,
                Audience = _jwtTokenConfig.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
