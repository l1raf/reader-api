using ReaderBackend.DTOs;
using ReaderBackend.Models;
using System;
using System.Threading.Tasks;

namespace ReaderBackend.Services
{
    public interface IUserService
    {
        Task<string> AddUser(User user);

        Task<User> GetUserById(Guid id);

        Task<User> GetUser(UserAuthDto user);
    }
}