using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;
using Xunit;

namespace Core.EntityFramework.IntegrationTests.Stores
{
  public class AuthorizationCodeStoreTests : TokenStoreTestsBase<AuthorizationCode>
  {
    public AuthorizationCodeStoreTests()
      : base()
    {
      _scopes = new[] {
        new Scope{ Name = "identity-scope", Type = ScopeType.Identity },
        new Scope{ Name = "resource-scope", Type = ScopeType.Resource },
      };
    }

    protected override BaseTokenStore<AuthorizationCode> CreateStore( IOperationalDbContext db, IdentityServer3.Core.Services.IScopeStore scopeStore, IdentityServer3.Core.Services.IClientStore clientStore )
    {
      return new AuthorizationCodeStore(db, scopeStore, clientStore);
    }

    protected override AuthorizationCode GenerateToken( string subject )
    {
      return new AuthorizationCode {
        Client = _client,
        Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
          new Claim(IdentityServer3.Core.Constants.ClaimTypes.Subject, subject),
        })),
        RequestedScopes = _scopes.Reverse(),
      };
    }

    protected override void AssertTokenMatches( AuthorizationCode original, AuthorizationCode fetched )
    {
      Assert.Equal(original.SubjectId, fetched.SubjectId);
      Assert.Same(original.Client, fetched.Client);
      Assert.Equal(original.RequestedScopes.OrderBy(s => s.Name), fetched.RequestedScopes.OrderBy(s => s.Name));
    }
  }
}
