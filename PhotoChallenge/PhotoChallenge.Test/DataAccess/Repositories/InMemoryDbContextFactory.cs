using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PhotoChallenge.DataAccess.Context;

namespace PhotoChallenge.Test.DataAccess.Repositories
{
    internal class InMemoryDbContextFactory
    {
        public DataContext GetDataContext()
        {
            var storeOptions = new OperationalStoreOptions(); ;
            var operationalStoreOptions = Options.Create(storeOptions);
            var options = new DbContextOptionsBuilder<DataContext>()
                            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                            .Options;
            var dbContext = new DataContext(options, operationalStoreOptions);

            return dbContext;
        }
    }
}
