using Microsoft.EntityFrameworkCore;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.DataAccess.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly DataContext _db;

        public AreaRepository(DataContext db)
        {
            _db = db;
        }

        public async Task<List<Area>> GetAreas()
        {
            return await _db.Area.ToListAsync();
        }

        public async Task<List<Area>> GetActualAreas()
        {
            return await _db.Area.Include(_ => _.Challenges).Where(_ => _.Challenges.Where(c => c.Status == ChallengeStatus.Live.ToString()).Any()).ToListAsync();
        }

        public async Task<Area> FindArea(int id)
        {
            var area = await _db.Area.SingleOrDefaultAsync(_ => _.Id == id);

            if (area is null)
                throw new AppException(ErrorCode.EntityNotFound, "Area is not found");

            return area;
        }

        public async Task<Area> AddArea(Area data)
        {
            var areaWithSameName = await _db.Area.SingleOrDefaultAsync(_ => _.Name == data.Name);

            if (areaWithSameName is not null)
                throw new AppException(ErrorCode.EntityValidationError, "Area with the same name already exists");

            await _db.Area.AddAsync(data);

            await _db.SaveChangesAsync();

            return await Task.FromResult(data);
        }

        public async Task DeleteArea(int id)
        {
            var area = await _db.Area.Include(_ => _.Challenges).SingleOrDefaultAsync(_ => _.Id == id);

            if (area is null)
                throw new AppException(ErrorCode.EntityNotFound, "Area is not found");

            if (area.Challenges.Any())
                throw new AppException(ErrorCode.EntityCannotBeDeleted, "Area cannot be deleted because of active challenges");

            _db.Entry(area).State = EntityState.Detached;

            _db.Area.Remove(new Area { Id = id });

            await _db.SaveChangesAsync();
        }
    }
}
