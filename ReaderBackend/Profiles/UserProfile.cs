using AutoMapper;
using ReaderBackend.DTOs;
using ReaderBackend.Models;

namespace ReaderBackend.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserUpdateDto, User>();
        }
    }
}
