using PhotoChallenge.Domain.DTO.UserInteraction;

namespace PhotoChallenge.BusinessLogic.Services.Interfaces
{
    public interface IUserInteractionService
    {
        Task<List<UserInteractionDto>> GetUserInteractions();
        Task<List<UserInteractionDto>> GetMyInteractions();
        Task<UserInteractionDto> AddUserInteraction(CreateUserInteractionDto data);
        Task<UserInteractionDto> UpdateUserInteraction(UpdateUserInteractionDto data);
        Task DeleteUserInteraction(int id);
    }
}
