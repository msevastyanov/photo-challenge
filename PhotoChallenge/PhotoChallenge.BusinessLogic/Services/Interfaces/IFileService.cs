using Microsoft.AspNetCore.Http;
using PhotoChallenge.Domain.DTO.Challenge;

namespace PhotoChallenge.BusinessLogic.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFile(UploadFileDto file);
        void DeleteFile(string fileName);
    }
}
