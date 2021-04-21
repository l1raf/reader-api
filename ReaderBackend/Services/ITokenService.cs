using ReaderBackend.DTOs;
using System.Threading.Tasks;

namespace ReaderBackend.Services
{
    public interface ITokenService
    {
        Task<(string error, UserAuthResponse response)> Authenticate(UserAuthDto userDto);

        Task<(string error, UserAuthResponse response)> RefreshToken(TokenRequest tokenRequest);
    }
}
