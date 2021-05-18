using ReaderBackend.DTOs;
using ReaderBackend.Models;
using ReaderBackend.Repositories;
using ReaderBackend.Utils;
using System;
using System.Threading.Tasks;

namespace ReaderBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> AddUser(User user)
        {
            string error = CredentialsHelper.AreValidUserCredentials(user.Login, user.Password);

            if (string.IsNullOrEmpty(error))
                error = await _userRepository.AddUser(user);

            return error;
        }

        public async Task<string> UpdateUser(User user)
        {
            string error = null;

            try
            {
                if (!(await _userRepository.SaveChanges()))
                    error = "Failed to save changes.";
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return error;
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<User> GetUser(UserAuthDto user)
        {
            return await _userRepository.GetUser(user);
        }
    }
}