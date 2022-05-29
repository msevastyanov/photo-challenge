using AutoMapper;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.DTO.Area;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.BusinessLogic.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IMapper _mapper;

        public AreaService(IAreaRepository areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        public async Task<List<AreaDto>> GetAreas()
        {
            var areas = await _areaRepository.GetAreas();

            return areas.Select(_ => _mapper.Map<AreaDto>(_)).ToList();
        }

        public async Task<List<AreaDto>> GetActualAreas()
        {
            var areas = await _areaRepository.GetActualAreas();

            return areas.Select(_ => _mapper.Map<AreaDto>(_)).ToList();
        }

        public async Task<AreaDto> AddArea(AreaDto data)
        {
            var newArea = _mapper.Map<Area>(data);

            var createdArea = await _areaRepository.AddArea(newArea);

            return _mapper.Map<AreaDto>(createdArea);
        }

        public async Task DeleteArea(int id)
        {
            await _areaRepository.DeleteArea(id);
        }
    }
}
