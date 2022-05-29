using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.DTO.User;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;
using System.Security.Claims;

namespace PhotoChallenge.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IUserInteractionRepository _userInteractionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            IUserInteractionRepository userInteractionRepository,
            UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userInteractionRepository = userInteractionRepository;
            _userManager = userManager;
        }

        public string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new AppException(ErrorCode.UserNotFound, "User id is null");

            return userId;
        }

        public async Task<ApplicationUser> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new AppException(ErrorCode.UserNotFound, "User is not found");

            return user;
        }

        public async Task<UserDto> GetProfileData()
        {
            var userId = GetCurrentUserId();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new AppException(ErrorCode.UserNotFound, "User is not found");

            var roles = await _userManager.GetRolesAsync(user);
            var interactions = await _userInteractionRepository.GetUserInteractions(userId);

            return new UserDto
            {
                UserId = user.Id,
                Username = user.UserName,
                DefaultAreaId = user.DefaultAreaId,
                Challenges = interactions.Where(_ => _.Status == UserInteractionStatus.Approved.ToString()).Count(),
                Award = interactions.Where(_ => _.Status == UserInteractionStatus.Approved.ToString()).Sum(_ => _.Challenge.Award),
                Role = roles.Any() ? roles.First() : null
            };
        }

        public async Task<List<UserDto>> GetUsers()
        {
            var applicationUsers = _userManager.Users.ToList();
            var users = new List<UserDto>();

            foreach (var user in applicationUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                users.Add(new UserDto
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    DefaultAreaId = user.DefaultAreaId,
                    Role = roles.Any() ? roles.First() : null
                });
            }

            return users;
        }

        public async Task<ApplicationUser> UpdateUser(UpdateUserDto data)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new AppException(ErrorCode.UserNotFound, "User id is null");

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new AppException(ErrorCode.UserNotFound, "User is not found");

            user.DefaultAreaId = data.DefaultAreaId;

            await _userManager.UpdateAsync(user);

            return user;
        }

        public async Task<ApplicationUser> ChangeRole(ChangeUserRoleDto data)
        {
            var userId = GetCurrentUserId();
            if (string.Equals(data.UserId, userId))
                throw new AppException(ErrorCode.EntityValidationError, "You cannot change your own role");

            var user = await _userManager.FindByIdAsync(data.UserId);

            if (user is null)
                throw new AppException(ErrorCode.UserNotFound, "User is not found");

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRolesAsync(user, new string[] { data.Role });

            await _userManager.AddClaimAsync(user, claim: new Claim(ClaimTypes.Role.ToString(), data.Role));

            return user;
        }
    }
}
