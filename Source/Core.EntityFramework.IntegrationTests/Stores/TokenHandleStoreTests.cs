using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;
using Xunit;

namespace Core.EntityFramework.IntegrationTests.Stores
{
  public class TokenHandleStoreTests : TokenStoreTestsBase<Token>
  {
    protected override BaseTokenStore<Token> CreateStore( IOperationalDbContext db, IdentityServer3.Core.Services.IScopeStore scopeStore, IdentityServer3.Core.Services.IClientStore clientStore )
    {
      return new TokenHandleStore(db, scopeStore, clientStore);
    }

    protected override Token GenerateToken( string subject )
    {
      return new Token {
        Client = _client,
        Lifetime = 3600,
        Claims = new List<Claim> {
          new Claim(IdentityServer3.Core.Constants.ClaimTypes.Subject, subject),
          new Claim(IdentityServer3.Core.Constants.ClaimTypes.Scope, "test-scope"),
        },
      };
    }

    protected override void AssertTokenMatches( Token original, Token fetched )
    {
      Assert.Equal(original.SubjectId, fetched.SubjectId);
      Assert.Same(_client, fetched.Client);
    }
  }
}
