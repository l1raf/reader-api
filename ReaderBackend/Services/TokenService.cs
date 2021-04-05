using ReaderBackend.DTOs;
using ReaderBackend.Jwt;
using ReaderBackend.Models;
using ReaderBackend.Repositories;

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

        public UserAuthResponse Authenticate(UserAuthDto userDto)
        {
            User user = _userRepository.GetUser(userDto);

            if (user == null)
                return null;

            return new UserAuthResponse
            {
                Id = user.Id,
                AccessToken = _jwtGenerator.CreateToken(user)
            };
        }
    }
}