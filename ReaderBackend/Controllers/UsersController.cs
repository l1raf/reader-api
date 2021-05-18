using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReaderBackend.DTOs;
using ReaderBackend.Models;
using ReaderBackend.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReaderBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public UsersController(IHttpContextAccessor contextAccessor, IUserService userService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
            _contextAccessor = contextAccessor;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserRegisterDto user)
        {
            var model = _mapper.Map<User>(user);
            string error = await _userService.AddUser(model);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update(UserUpdateDto userDto)
        {
            string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return BadRequest();

            User user = await _userService.GetUserById(Guid.Parse((ReadOnlySpan<char>)userId));

            if (user is null) 
                return NotFound();

            _mapper.Map(userDto, user);

            string error = await _userService.UpdateUser(user);

            if (error is not null)
                return BadRequest();

            return NoContent();
        }
    }
}