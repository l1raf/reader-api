using AutoMapper;
using ReaderBackend.Dtos;
using ReaderBackend.Models;

namespace ReaderBackend.Profiles
{
    public class WebPageProfile : Profile
    {
        public WebPageProfile()
        {
            CreateMap<WebPage, WebPageReadDto>();
            CreateMap<WebPageCreateDto, WebPage>();
            CreateMap<WebPageUpdateDto, WebPage>();
            CreateMap<WebPage, WebPageUpdateDto>();
        }
    }
}
