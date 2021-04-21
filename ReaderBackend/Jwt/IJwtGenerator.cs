using ReaderBackend.DTOs;
using ReaderBackend.Models;
using System.Threading.Tasks;

namespace ReaderBackend.Jwt
{
    public interface IJwtGenerator
    {
        Task<(string error, UserAuthResponse response)> CreateToken(User user);

        Task<(string error, UserAuthResponse response)> RefreshToken(TokenRequest tokenRequest);
    }
}