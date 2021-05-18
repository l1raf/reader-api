using ReaderBackend.DTOs;
using ReaderBackend.Models;
using System;
using System.Threading.Tasks;

namespace ReaderBackend.Repositories
{
    public interface IUserRepository
    { 
        Task<string> AddUser(User user);

        Task<User> GetUserById(Guid id);

        Task<User> GetUser(UserAuthDto user);

        Task<bool> SaveChanges();
    }
}