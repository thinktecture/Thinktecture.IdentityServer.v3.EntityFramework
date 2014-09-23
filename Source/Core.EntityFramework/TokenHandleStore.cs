using System;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Connect.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class TokenHandleStore : BaseTokenStore<Token>, ITokenHandleStore
    {
        public TokenHandleStore(CoreDbContextFactoryBase dbFactory)
            : base(dbFactory, Entities.TokenType.TokenHandle)
        {
        }

        public override Task StoreAsync(string key, Token value)
        {
            using (var db = DbFactory.Create())
            {
                var efToken = new Entities.Token
                {
                    Key = key,
                    JsonCode = ConvertToJson(value),
                    Expiry = DateTime.UtcNow.AddSeconds(value.Lifetime),
                    TokenType = this.TokenType
                };

                db.Tokens.Add(efToken);
                db.SaveChanges();
            }
            
            return Task.FromResult(0);
        }
    }
}
