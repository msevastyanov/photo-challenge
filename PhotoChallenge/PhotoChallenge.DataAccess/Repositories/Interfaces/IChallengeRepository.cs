using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.DataAccess.Repositories.Interfaces
{
    public interface IChallengeRepository
    {
        Task<List<Challenge>> GetChallenges();
        Task<List<Challenge>> GetLiveChallenges();
        Task<Challenge> FindChallenge(int id);
        Task<Challenge> AddChallenge(Challenge challenge);
        Task<Challenge> UpdateChallenge(Challenge challenge);
    }
}
