using PhotoChallenge.Domain.DTO.Challenge;

namespace PhotoChallenge.BusinessLogic.Services.Interfaces
{
    public interface IChallengeService
    {
        Task<List<ChallengeDto>> GetChallenges();
        Task<List<ChallengeDto>> GetLiveChallenges();
        Task<ChallengeDto> GetChallenge(int id);
        Task<ChallengeDto> AddChallenge(ChallengeDto data);
        Task<ChallengeDto> UpdateChallenge(ChallengeDto data);
    }
}
