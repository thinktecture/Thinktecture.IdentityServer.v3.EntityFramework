using IdentityServer3.EntityFramework;
using IdentityServer3.EntityFramework.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;

namespace Core.EntityFramework.IntegrationTests
{
    [TestClass]
    public class ClientConfigurationDbContextTests
    {
        private const string ConfigConnectionStringName = "Config";

        public ClientConfigurationDbContextTests()
        {
            Database.SetInitializer<ClientConfigurationDbContext>(
                new DropCreateDatabaseAlways<ClientConfigurationDbContext>());
        }

        [TestMethod]
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

                Assert.Equals(0, client.RedirectUris.Count());
            }
        }
    }
}