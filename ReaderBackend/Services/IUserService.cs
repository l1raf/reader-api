using System;
using ReaderBackend.DTOs;
using ReaderBackend.Models;

namespace ReaderBackend.Services
{
    public interface IUserService
    {
        string AddUser(User user);

        User GetUserById(Guid id);

        User GetUser(UserAuthDto user);
    }
}