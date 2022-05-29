using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.DataAccess.Repositories;
using PhotoChallenge.DataAccess.Repositories.Interfaces;

namespace PhotoChallenge.DataAccess.Configuration
{
    public class DbConfig
    {
        public static string? ConnectionString { get; private set; }

        public static void ConfigureDb(IServiceCollection services, string connectionString)
        {
            ConnectionString = connectionString;

            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly("PhotoChallenge.DataAccess")));

            services.AddTransient<IChallengeRepository, ChallengeRepository>();
            services.AddTransient<IAreaRepository, AreaRepository>();
            services.AddTransient<IUserInteractionRepository, UserInteractionRepository>();
        }
    }
}
