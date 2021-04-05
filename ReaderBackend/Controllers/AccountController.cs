using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReaderBackend.DTOs;
using ReaderBackend.Models;
using ReaderBackend.Services;
using ReaderBackend.Utils;

namespace ReaderBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountController(ITokenService tokenService, IUserService userService, IMapper mapper)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost("register")]
        public ActionResult<UserAuthResponse> Register(UserRegisterDto user)
        {
            var model = _mapper.Map<User>(user);
            string error = _userService.AddUser(model);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            return Ok();
        }

        [HttpPost("authenticate")]
        public ActionResult<UserAuthResponse> Authenticate(UserAuthDto userAuthDto)
        {
            string error = CredentialsHelper.AreValidUserCredentials(userAuthDto.Login, userAuthDto.Password);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = _tokenService.Authenticate(userAuthDto);

            return Ok(response);
        }
    }
}