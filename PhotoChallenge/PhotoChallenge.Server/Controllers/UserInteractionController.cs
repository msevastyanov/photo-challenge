using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.Domain.DTO.UserInteraction;

namespace PhotoChallenge.Server.Controllers
{
    [Authorize]
    [Route("api/interaction")]
    [ApiController]
    public class UserInteractionController : ControllerBase
    {
        private readonly IUserInteractionService _userInteractionService;

        public UserInteractionController(IUserInteractionService userInteractionService)
        {
            _userInteractionService = userInteractionService;
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public async Task<IEnumerable<UserInteractionDto>> Get()
        {
            return await _userInteractionService.GetUserInteractions();
        }

        [HttpGet("my")]
        public async Task<IEnumerable<UserInteractionDto>> GetMy()
        {
            return await _userInteractionService.GetMyInteractions();
        }

        [HttpPost]
        public async Task<UserInteractionDto> Create([FromBody] CreateUserInteractionDto model)
        {
            return await _userInteractionService.AddUserInteraction(model);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost("update")]
        public async Task<UserInteractionDto> Update([FromBody] UpdateUserInteractionDto model)
        {
            return await _userInteractionService.UpdateUserInteraction(model);
        }

        [HttpPost("delete/{id}")]
        public async Task Delete(int id)
        {
            await _userInteractionService.DeleteUserInteraction(id);
        }
    }
}
