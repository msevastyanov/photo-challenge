using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PhotoChallenge.DataAccess.Configuration;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.DataAccess.Context
{
    public class DataContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
           : base(options, operationalStoreOptions)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Challenge>()
                .Property(_ => _.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Area>()
                .Property(_ => _.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<UserInteraction>()
                .Property(_ => _.Id)
                .ValueGeneratedOnAdd();
        }

        public DbSet<Challenge> Challenge { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<UserInteraction> UserInteraction { get; set; }
    }
}
