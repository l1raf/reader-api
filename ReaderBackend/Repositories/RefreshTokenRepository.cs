using Microsoft.EntityFrameworkCore;
using ReaderBackend.Context;
using ReaderBackend.Models;
using System.Threading.Tasks;

namespace ReaderBackend.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ReaderContext _context;

        public RefreshTokenRepository(ReaderContext context)
        {
            _context = context;
        }

        public async Task<string> AddToken(RefreshToken token)
        {
            string error = null;
            await _context.RefreshTokens.AddAsync(token);
            
            if (!(await _context.SaveChangesAsync() >= 0))
                error = "Failed to save refresh token";
            
            return error;
        }

        public async Task<RefreshToken> GetToken(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task UpdateToken(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}