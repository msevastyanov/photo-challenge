using PhotoChallenge.Domain.DTO.User;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        string GetCurrentUserId();
        Task<ApplicationUser> GetUser(string userId);
        Task<UserDto> GetProfileData();
        Task<List<UserDto>> GetUsers();
        Task<ApplicationUser> UpdateUser(UpdateUserDto data);
        Task<ApplicationUser> ChangeRole(ChangeUserRoleDto data);
    }
}
