using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.Domain.DTO.Challenge;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;

namespace PhotoChallenge.BusinessLogic.Services
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _hostingEnvironment;

        public FileService(IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> UploadFile(UploadFileDto file)
        {
            try
            {
                const string dirname = "uploads";
                var contentRootPath = _hostingEnvironment.ContentRootPath;
                var dirPath = Path.Combine(contentRootPath, "wwwroot", dirname);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                var fileName = Path.Combine(Path.GetRandomFileName() + Path.GetExtension(file.Name));
                var path = Path.Combine(dirPath, fileName);

                var fileBytes = Convert.FromBase64String(file.Content);

                await File.WriteAllBytesAsync(path, fileBytes);

                return fileName;
            }
            catch (Exception ex)
            {
                throw new AppException(ErrorCode.EntityValidationError, ex.Message);
            }

        }

        public void DeleteFile(string fileName)
        {
            const string dirname = "uploads";
            var contentRootPath = _hostingEnvironment.ContentRootPath;
            var dirPath = Path.Combine(contentRootPath, "wwwroot", dirname);

            File.Delete(Path.Combine(dirPath, fileName));
        }
    }
}
