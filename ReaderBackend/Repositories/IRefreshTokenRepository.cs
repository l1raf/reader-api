using ReaderBackend.Models;
using System.Threading.Tasks;

namespace ReaderBackend.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<string> AddToken(RefreshToken token);

        Task<RefreshToken> GetToken(string token);

        Task UpdateToken(RefreshToken token);
    }
}