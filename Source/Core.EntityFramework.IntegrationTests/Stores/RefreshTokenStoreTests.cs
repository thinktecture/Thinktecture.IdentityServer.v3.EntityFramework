using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;
using Xunit;

namespace Core.EntityFramework.IntegrationTests.Stores
{
  public class RefreshTokenStoreTests : TokenStoreTestsBase<RefreshToken>
  {
    protected override BaseTokenStore<RefreshToken> CreateStore( IOperationalDbContext db, IdentityServer3.Core.Services.IScopeStore scopeStore, IdentityServer3.Core.Services.IClientStore clientStore )
    {
      return new RefreshTokenStore(db, scopeStore, clientStore);
    }

    protected override RefreshToken GenerateToken( string subject )
    {
      return new RefreshToken {
        AccessToken = new Token {
          Client = _client,
          Lifetime = 3600,
          Claims = new List<Claim> {
            new Claim(IdentityServer3.Core.Constants.ClaimTypes.Subject, subject),
            new Claim(IdentityServer3.Core.Constants.ClaimTypes.Scope, "test-scope"),
          },
        },
        CreationTime = DateTimeOffset.UtcNow,
        LifeTime = 3600,
      };
    }

    protected override void AssertTokenMatches( RefreshToken original, RefreshToken fetched )
    {
      Assert.Equal(original.SubjectId, fetched.SubjectId);
      Assert.Equal(_client, fetched.AccessToken.Client);
    }
  }
}
