using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.EntityFramework;
using Xunit;

namespace Core.EntityFramework.IntegrationTests.Stores
{
  public abstract class TokenStoreTestsBase<T> where T : class
  {
    private const string OperationalConnectionStringName = "Operational";

    protected Client _client;
    protected IEnumerable<Scope> _scopes;

    public TokenStoreTestsBase()
    {
      Database.SetInitializer<OperationalDbContext>(
          new DropCreateDatabaseAlways<OperationalDbContext>());

      _client = new Client {
        ClientId = "123",
        Enabled = true,
        AbsoluteRefreshTokenLifetime = 5,
        AccessTokenLifetime = 10,
        AccessTokenType = AccessTokenType.Jwt,
        AllowRememberConsent = true,
        RedirectUris = new List<string> { "http://foo.com" }
      };

      _scopes = new Scope[0];
    }

    protected abstract BaseTokenStore<T> CreateStore(IOperationalDbContext db, IScopeStore scopeStore, IClientStore clientStore);

    protected abstract T GenerateToken( string subject );

    protected abstract void AssertTokenMatches( T original, T fetched );

    [Fact]
    public async Task CanPutAndGetAToken()
    {
      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        var clientStore = new InMemoryClientStore(new Client[] { _client });
        var scopeStore = new InMemoryScopeStore(_scopes);
        var store = CreateStore(db, scopeStore, clientStore);

        var key = typeof(T).Name;
        var token = GenerateToken(String.Format("test-subject-{0}", key));

        await store.StoreAsync(key, token);
        var result = await store.GetAsync(key);

        Assert.IsType<T>(result);
        AssertTokenMatches(token, result);
      }
    }

    [Fact]
    public async Task CanFetchAllTokens()
    {
      using( var db = new OperationalDbContext(OperationalConnectionStringName) ) {
        var client = new Client {
          ClientId = "123",
          Enabled = true,
          AbsoluteRefreshTokenLifetime = 5,
          AccessTokenLifetime = 10,
          AccessTokenType = AccessTokenType.Jwt,
          AllowRememberConsent = true,
          RedirectUris = new List<string> { "http://foo.com" }
        };
        var clientStore = new InMemoryClientStore(new Client[]{client});
        var scopeStore = new InMemoryScopeStore(_scopes);
        var store = CreateStore(db, scopeStore, clientStore);

        var key = typeof(T).Name;
        var subject = String.Format("good-subject-{0}", key);
        for( int i = 0; i < 10; i++ ) {
          var token = GenerateToken(subject);
          await store.StoreAsync(String.Format("{0}-good-{1}", key, i), token);
        }
        for( int i = 0; i < 5; i++ ) {
          var token = GenerateToken(String.Format("bad-subject-{0}", key));
          await store.StoreAsync(String.Format("{0}-bad-{1}", key, i), token);
        }

        var result = await store.GetAllAsync(subject);

        Assert.Equal(10, result.Count());
        Assert.All(result, t => {
          Assert.Equal(subject, t.SubjectId);
          Assert.Equal(client.ClientId, t.ClientId);
        });
      }
    }
  }
}
