using System.Data.Entity;
using System.Threading.Tasks;
using IdentityServer3.EntityFramework;
using IdentityServer3.EntityFramework.Entities;
using Xunit;

namespace Core.EntityFramework.IntegrationTests
{
  public class OperationalConfigurationDbContextTests
  {
    private const string OperationalConnectionStringName = "Operational";

    public OperationalConfigurationDbContextTests()
    {
      Database.SetInitializer<OperationalDbContext>(
          new DropCreateDatabaseAlways<OperationalDbContext>());
    }

    [Fact]
    public async Task CanAddAndDeleteTokens()
    {
      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        db.Tokens.Add(new Token {
          ClientId = "test-client",
          SubjectId = "test-token-subject",
          Key = "token-key",
          TokenType = IdentityServer3.EntityFramework.Entities.TokenType.AuthorizationCode,
          JsonCode = "{\"json\":\"fake\"}",
        });

        await db.SaveChangesAsync();
      }

      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        var token = await db.Tokens.FirstAsync(t => t.SubjectId == "test-token-subject");
        Assert.Equal("token-key", token.Key);

        db.Tokens.Remove(token);
        await db.SaveChangesAsync();
      }

      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        var token = await db.Tokens.FirstOrDefaultAsync(t => t.SubjectId == "test-token-subject");

        Assert.Null(token);
      }
    }

    [Fact]
    public async Task CanAddUpdateAndDeleteConsents()
    {
      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        db.Consents.Add(new Consent {
          ClientId = "test-client",
          Subject = "test-consent-subject",
          Scopes = "foo",
        });

        await db.SaveChangesAsync();
      }

      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        var consent = await db.Consents.FirstAsync(t => t.Subject == "test-consent-subject");
        Assert.Equal("foo", consent.Scopes);

        consent.Scopes += ",bar";
        await db.SaveChangesAsync();
      }

      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        var consent = await db.Consents.FirstAsync(t => t.Subject == "test-consent-subject");
        Assert.Equal("foo,bar", consent.Scopes);

        db.Consents.Remove(consent);
        await db.SaveChangesAsync();
      }

      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        var consent = await db.Consents.FirstOrDefaultAsync(t => t.Subject == "test-consent-subject");

        Assert.Null(consent);
      }
    }
  }
}
