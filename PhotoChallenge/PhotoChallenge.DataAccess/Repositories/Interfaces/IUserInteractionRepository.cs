using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.DataAccess.Repositories.Interfaces
{
    public interface IUserInteractionRepository
    {
        Task<List<UserInteraction>> GetUserInteractions();
        Task<List<UserInteraction>> GetUserInteractions(string userId);
        Task<UserInteraction> FindUserInteraction(int id);
        Task<UserInteraction> FindUserInteraction(int challengeId, string userId);
        Task<UserInteraction> AddUserInteraction(UserInteraction data);
        Task<UserInteraction> UpdateUserInteraction(UserInteraction data);
        Task DeleteUserInteraction(int id);
    }
}
