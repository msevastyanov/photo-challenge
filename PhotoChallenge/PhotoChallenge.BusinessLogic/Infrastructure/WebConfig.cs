using Microsoft.Extensions.DependencyInjection;
using PhotoChallenge.BusinessLogic.Infrastructure.Mappers;
using PhotoChallenge.BusinessLogic.Services;
using PhotoChallenge.BusinessLogic.Services.Interfaces;

namespace PhotoChallenge.BusinessLogic.Infrastructure
{
    public static class WebConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddTransient<IChallengeService, ChallengeService>();
            services.AddTransient<IAreaService, AreaService>();
            services.AddTransient<IUserInteractionService, UserInteractionService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}
