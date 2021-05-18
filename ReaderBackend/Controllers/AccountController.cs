using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReaderBackend.DTOs;
using ReaderBackend.Services;
using ReaderBackend.Utils;

namespace ReaderBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AccountController(ITokenService tokenService)
        {
            _tokenService = tokenService;
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