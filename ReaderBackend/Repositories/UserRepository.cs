using Microsoft.EntityFrameworkCore;
using ReaderBackend.Context;
using ReaderBackend.DTOs;
using ReaderBackend.Models;
using System;
using System.Threading.Tasks;

namespace ReaderBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ReaderContext _context;

        public UserRepository(ReaderContext context)
        {
            _context = context;
        }

        public async Task<string> AddUser(User user)
        {
            if (await _context.Users.AnyAsync(x => x.Login == user.Login))
                return "User with such email already exists.";

            await _context.Users.AddAsync(user);

            if (await _context.SaveChangesAsync() < 0)
                return "Failed to add user.";

            return null;
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUser(UserAuthDto user)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Login == user.Login && x.Password == user.Password);
        }
    }
}