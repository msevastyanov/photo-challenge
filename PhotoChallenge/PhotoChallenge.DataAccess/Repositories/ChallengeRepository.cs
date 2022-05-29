using Microsoft.EntityFrameworkCore;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;
using PhotoChallenge.Domain.Enums;

namespace PhotoChallenge.DataAccess.Repositories
{
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly DataContext _db;

        public ChallengeRepository(DataContext db)
        {
            _db = db;
        }

        public async Task<List<Challenge>> GetChallenges()
        {
            return await _db.Challenge.Include(_ => _.Area).ToListAsync();
        }

        public async Task<List<Challenge>> GetLiveChallenges()
        {
            return await _db.Challenge.Include(_ => _.Area).Where(_ => _.Status == ChallengeStatus.Live.ToString()).ToListAsync();
        }

        public async Task<Challenge> FindChallenge(int id)
        {
            var challenge = await _db.Challenge.Include(_ => _.Area).SingleOrDefaultAsync(_ => _.Id == id);

            if (challenge is null)
                throw new AppException(ErrorCode.EntityNotFound, "Challenge is not found");

            return challenge;
        }

        public async Task<Challenge> AddChallenge(Challenge data)
        {
            await _db.Challenge.AddAsync(data);

            await _db.SaveChangesAsync();

            return await Task.FromResult(data);
        }

        public async Task<Challenge> UpdateChallenge(Challenge data)
        {
            var existingChallenge = await _db.Challenge.SingleOrDefaultAsync(_ => _.Id == data.Id);

            if (existingChallenge is null)
                throw new AppException(ErrorCode.EntityNotFound, "Challenge is not found");

            _db.Entry(existingChallenge).State = EntityState.Detached;

            _db.Update(data);

            await _db.SaveChangesAsync();

            return await Task.FromResult(data);
        }
    }
}
