using System.Threading.Tasks;
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
        public async Task<ActionResult> Register(UserRegisterDto user)
        {
            var model = _mapper.Map<User>(user);
            string error = await _userService.AddUser(model);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            return Ok();
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(UserAuthDto userAuthDto)
        {
            string error = CredentialsHelper.AreValidUserCredentials(userAuthDto.Login, userAuthDto.Password);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var result = await _tokenService.Authenticate(userAuthDto);

            if (result == (null, null))
                return NotFound();

            if (result.error != null)
                return BadRequest(result.error);

            return Ok(result.response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            var result = await _tokenService.RefreshToken(tokenRequest);

            if (result.error != null)
                return BadRequest(result.error);

            return Ok(result.response);
        }
    }
}