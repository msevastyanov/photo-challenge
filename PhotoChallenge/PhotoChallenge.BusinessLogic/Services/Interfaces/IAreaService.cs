using PhotoChallenge.Domain.DTO.Area;

namespace PhotoChallenge.BusinessLogic.Services.Interfaces
{
    public interface IAreaService
    {
        Task<List<AreaDto>> GetAreas();
        Task<List<AreaDto>> GetActualAreas();
        Task<AreaDto> AddArea(AreaDto data);
        Task DeleteArea(int id);
    }
}
