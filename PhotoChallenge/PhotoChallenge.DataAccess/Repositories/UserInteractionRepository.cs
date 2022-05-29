using Microsoft.EntityFrameworkCore;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.DataAccess.Repositories
{
    public class UserInteractionRepository : IUserInteractionRepository
    {
        private readonly DataContext _db;

        public UserInteractionRepository(DataContext db)
        {
            _db = db;
        }

        public async Task<List<UserInteraction>> GetUserInteractions()
        {
            return await _db.UserInteraction.Include(_ => _.Challenge).ThenInclude(_ => _.Area).Include(_ => _.User).ToListAsync();
        }

        public async Task<List<UserInteraction>> GetUserInteractions(string userId)
        {
            return await _db.UserInteraction.Include(_ => _.Challenge).ThenInclude(_ => _.Area).Include(_ => _.User).Where(_ => _.User.Id == userId).ToListAsync();
        }

        public async Task<UserInteraction> FindUserInteraction(int id)
        {
            var interaction = await _db.UserInteraction.Include(_ => _.Challenge).ThenInclude(_ => _.Area).Include(_ => _.User).SingleOrDefaultAsync(_ => _.Id == id);

            if (interaction is null)
                throw new AppException(ErrorCode.EntityNotFound, "User interaction is not found");

            return interaction;
        }

        public async Task<UserInteraction> FindUserInteraction(int challengeId, string userId)
        {
            var interaction = await _db.UserInteraction
                .Include(_ => _.Challenge)
                .ThenInclude(_ => _.Area)
                .Include(_ => _.User)
                .SingleOrDefaultAsync(_ => _.Challenge.Id == challengeId && _.User.Id == userId);

            return interaction;
        }

        public async Task<UserInteraction> AddUserInteraction(UserInteraction data)
        {
            var existingInteraction = await _db.UserInteraction
                .Include(_ => _.Challenge)
                .Include(_ => _.User)
                .SingleOrDefaultAsync(_ => _.User.Id == data.User.Id && _.Challenge.Id == data.Challenge.Id);

            if (existingInteraction is not null)
                throw new AppException(ErrorCode.EntityValidationError, "You have already taken part in this challenge");

            await _db.UserInteraction.AddAsync(data);

            await _db.SaveChangesAsync();

            return await Task.FromResult(data);
        }

        public async Task<UserInteraction> UpdateUserInteraction(UserInteraction data)
        {
            _db.Update(data);

            await _db.SaveChangesAsync();

            return await Task.FromResult(data);
        }

        public async Task DeleteUserInteraction(int id)
        {
            var interaction = await _db.UserInteraction.SingleOrDefaultAsync(_ => _.Id == id);

            if (interaction is null)
                throw new AppException(ErrorCode.EntityNotFound, "User interaction is not found");

            if (interaction.Status != UserInteractionStatus.Pending.ToString())
                throw new AppException(ErrorCode.EntityCannotBeDeleted, "User interaction cannot be deleted because of status");

            _db.Entry(interaction).State = EntityState.Detached;

            _db.UserInteraction.Remove(new UserInteraction { Id = id });

            await _db.SaveChangesAsync();
        }
    }
}
