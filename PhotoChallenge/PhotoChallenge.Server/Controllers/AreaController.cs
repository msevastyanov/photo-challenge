using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.Domain.DTO.Area;

namespace PhotoChallenge.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public async Task<IEnumerable<AreaDto>> Get()
        {
            return await _areaService.GetAreas();
        }

        [HttpGet("actual")]
        public async Task<IEnumerable<AreaDto>> GetActual()
        {
            return await _areaService.GetActualAreas();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<AreaDto> Create([FromBody] AreaDto model)
        {
            return await _areaService.AddArea(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id}")]
        public async Task Delete(int id)
        {
            await _areaService.DeleteArea(id);
        }
    }
}
