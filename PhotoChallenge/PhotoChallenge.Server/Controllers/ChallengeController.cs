using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.Domain.DTO.Challenge;

namespace PhotoChallenge.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly IChallengeService _challengeService;

        public ChallengeController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        [HttpGet]
        public async Task<IEnumerable<ChallengeDto>> Get()
        {
            return await _challengeService.GetChallenges();
        }

        [AllowAnonymous]
        [HttpGet("live")]
        public async Task<IEnumerable<ChallengeDto>> GetLive()
        {
            return await _challengeService.GetLiveChallenges();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChallengeDto>> Get(int id)
        {
            return await _challengeService.GetChallenge(id);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public async Task<ChallengeDto> Create([FromBody] ChallengeDto model)
        {
            return await _challengeService.AddChallenge(model);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost("update")]
        public async Task<ChallengeDto> Update([FromBody] ChallengeDto model)
        {
            return await _challengeService.UpdateChallenge(model);
        }
    }
}
