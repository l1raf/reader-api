using ReaderBackend.DTOs;

namespace ReaderBackend.Services
{
    public interface ITokenService
    {
        UserAuthResponse Authenticate(UserAuthDto userDto);
    }
}
