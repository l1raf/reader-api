using System;
using System.Linq;
using ReaderBackend.Context;
using ReaderBackend.DTOs;
using ReaderBackend.Models;

namespace ReaderBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ReaderContext _context;

        public UserRepository(ReaderContext context)
        {
            _context = context;
        }

        public string AddUser(User user)
        {
            if (_context.Users.Any(x => x.Login == user.Login))
                return "User with such login already exists.";

            _context.Users.Add(user);

            if (_context.SaveChanges() < 0)
                return "Failed to add user.";

            return null;
        }

        public User GetUserById(Guid id)
        {
            return _context.Users.Find(id);
        }

        public User GetUser(UserAuthDto user)
        {
            return _context.Users.SingleOrDefault(x => x.Login == user.Login && x.Password == user.Password);
        }
    }
}