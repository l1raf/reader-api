using ReaderBackend.DTOs;
using ReaderBackend.Jwt;
using ReaderBackend.Repositories;
using System.Threading.Tasks;

namespace ReaderBackend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IUserRepository _userRepository;

        public TokenService(IUserRepository userRepository, IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
            _userRepository = userRepository;
        }

        public async Task<(string, UserAuthResponse)> Authenticate(UserAuthDto userDto)
        {
            var user = await _userRepository.GetUser(userDto);

            if (user == null)
                return (null, null);

            return await _jwtGenerator.CreateToken(user);
        }

        public async Task<(string, UserAuthResponse)> RefreshToken(TokenRequest tokenRequest)
        {
            return await _jwtGenerator.RefreshToken(tokenRequest);
        }
    }
}