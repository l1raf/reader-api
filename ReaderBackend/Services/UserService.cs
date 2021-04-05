using System;
using ReaderBackend.DTOs;
using ReaderBackend.Models;
using ReaderBackend.Repositories;
using ReaderBackend.Utils;

namespace ReaderBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string AddUser(User user)
        {
            string error = CredentialsHelper.AreValidUserCredentials(user.Login, user.Password);

            if (string.IsNullOrEmpty(error))
                error = _userRepository.AddUser(user);

            return error;
        }

        public User GetUserById(Guid id)
        {
            return _userRepository.GetUserById(id);
        }

        public User GetUser(UserAuthDto user)
        {
            return _userRepository.GetUser(user);
        }
    }
}