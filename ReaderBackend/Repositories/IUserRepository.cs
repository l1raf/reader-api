using System;
using ReaderBackend.DTOs;
using ReaderBackend.Models;

namespace ReaderBackend.Repositories
{
    public interface IUserRepository
    {
        string AddUser(User user);

        User GetUserById(Guid id);

        User GetUser(UserAuthDto user);
    }
}