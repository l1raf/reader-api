using Microsoft.IdentityModel.Tokens;
using ReaderBackend.DTOs;
using ReaderBackend.Models;
using ReaderBackend.Repositories;
using ReaderBackend.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReaderBackend.Jwt
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly IUserService _userService;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private static readonly Random random = new();

        public JwtGenerator(JwtTokenConfig config, IUserService userService,
            TokenValidationParameters tokenValidationParameters, IRefreshTokenRepository refreshTokenRepository)
        {
            _userService = userService;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenConfig = config;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<(string, UserAuthResponse)> CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
                Expires = DateTime.UtcNow.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(_jwtTokenConfig.RefreshTokenExpiration),
                Token = GetRandomString(35) + Guid.NewGuid()
            };

            try
            {
                await _refreshTokenRepository.AddToken(refreshToken);
            }
            catch (Exception)
            {
                return ("Failed to save token", null);
            }

            return (null, new UserAuthResponse()
            {
                Id = user.Id,
                AccessToken = accessToken,
                ExpiryDate = tokenDescriptor.Expires,
                RefreshToken = refreshToken.Token
            });
        }

        public async Task<(string, UserAuthResponse)> RefreshToken(TokenRequest tokenRequest)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                jwtSecurityTokenHandler.ValidateToken(tokenRequest.AccessToken, _tokenValidationParameters,
                    out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                        StringComparison.InvariantCulture);

                    if (result == false)
                        return ("Token has wrong schema", null);
                }

                return ("Token has not yet expired", null);
            }
            catch (SecurityTokenExpiredException)
            {
                var storedToken = await _refreshTokenRepository.GetToken(tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return ("Token does not exist", null);
                }

                if (storedToken.IsUsed)
                {
                    return ("Token has been used", null);
                }

                if (storedToken.IsRevoked)
                {
                    return ("Token has been revoked", null);
                }

                storedToken.IsUsed = true;
                await _refreshTokenRepository.UpdateToken(storedToken);

                User user = await _userService.GetUserById(storedToken.UserId);
                return await CreateToken(user);
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        private string GetRandomString(int length)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}