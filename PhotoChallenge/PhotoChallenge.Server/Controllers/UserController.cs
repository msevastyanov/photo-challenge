using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.Domain.DTO.User;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<UserDto> Get()
        {
            return await _userService.GetProfileData();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<List<UserDto>> GetUsers()
        {
            return await _userService.GetUsers();
        }

        [HttpPost]
        public async Task<ApplicationUser> Update([FromBody] UpdateUserDto model)
        {
            return await _userService.UpdateUser(model);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("role")]
        public async Task<ApplicationUser> ChangeRole([FromBody] ChangeUserRoleDto model)
        {
            return await _userService.ChangeRole(model);
        }
    }
}
