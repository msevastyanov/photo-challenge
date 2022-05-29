using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.Domain.DTO.Challenge;

namespace PhotoChallenge.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] UploadFileDto fileDto)
        {
            return await _fileService.UploadFile(fileDto);
        }
    }
}
