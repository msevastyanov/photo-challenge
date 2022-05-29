using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.DataAccess.Repositories.Interfaces
{
    public interface IAreaRepository
    {
        Task<List<Area>> GetAreas();
        Task<List<Area>> GetActualAreas();
        Task<Area> FindArea(int id);
        Task<Area> AddArea(Area area);
        Task DeleteArea(int id);
    }
}
