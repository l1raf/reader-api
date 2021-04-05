using ReaderBackend.Models;

namespace ReaderBackend.Jwt
{
    public interface IJwtGenerator
    {
        public string CreateToken(User user);
    }
}