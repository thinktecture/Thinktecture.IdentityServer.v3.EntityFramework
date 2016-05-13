using IdentityServer3.EntityFramework;
using IdentityServer3.EntityFramework.Entities;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.EntityFramework.IntegrationTests
{
    public class ClientConfigurationDbContextTests
    {
        private const string ConfigConnectionStringName = "Config";

        public ClientConfigurationDbContextTests()
        {
            Database.SetInitializer<ClientConfigurationDbContext>(
                new DropCreateDatabaseAlways<ClientConfigurationDbContext>());

            //Database.SetInitializer<OperationalDbContext>(
            //    new DropCreateDatabaseAlways<OperationalDbContext>());
        }

        [Fact]
        public void CleansUpDatabase()
        {
            var cleanUp = new TokenCleanup(new EntityFrameworkServiceOptions
            {
                ConnectionString = ConfigConnectionStringName
            },3);

            cleanUp.Start();

            Task.Delay(100000).Wait();
        }

        [Fact]
        public void CanAddAndDeleteClientScopes()
        {
            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                db.Clients.Add(new Client
                {
                    ClientId = "test-client-scopes",
                    ClientName = "Test Client"
                });

                db.SaveChanges();
            }

            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                var client = db.Clients.First();

                client.AllowedScopes.Add(new ClientScope
                {
                    Scope = "test"
                });

                db.SaveChanges();
            }

            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                var client = db.Clients.First();
                var scope = client.AllowedScopes.First();

                client.AllowedScopes.Remove(scope);

                db.SaveChanges();
            }

            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                var client = db.Clients.First();

                Assert.Equal(0, client.AllowedScopes.Count());
            }
        }

        [Fact]
        public void CanAddAndDeleteClientRedirectUri()
        {
            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                db.Clients.Add(new Client
                {
                    ClientId = "test-client",
                    ClientName = "Test Client"
                });

                db.SaveChanges();
            }

            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                var client = db.Clients.First();

                client.RedirectUris.Add(new ClientRedirectUri
                {
                    Uri = "https://redirect-uri-1"
                });

                db.SaveChanges();
            }

            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                var client = db.Clients.First();
                var redirectUri = client.RedirectUris.First();

                client.RedirectUris.Remove(redirectUri);

                db.SaveChanges();
            }

            using (var db = new ClientConfigurationDbContext(ConfigConnectionStringName))
            {
                var client = db.Clients.First();

                Assert.Equal(0, client.RedirectUris.Count());
            }
        }
    }
}