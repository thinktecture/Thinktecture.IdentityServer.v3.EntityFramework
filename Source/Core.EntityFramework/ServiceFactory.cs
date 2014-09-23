using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ServiceFactory
    {
        private readonly CoreDbContextFactoryBase _dbFactory;

        public ServiceFactory(CoreDbContextFactoryBase dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public IClientStore CreateClientStore()
        {
            return new ClientStore(_dbFactory);
        }

        public IScopeStore CreateScopeStore()
        {
            return new ScopeStore(_dbFactory);
        }

        public IConsentService CreateConsentService()
        {
            return new ConsentService(_dbFactory);
        }

        public IAuthorizationCodeStore CreateAuthorizationCodeStore()
        {
            return new AuthorizationCodeStore(_dbFactory);
        }

        public ITokenHandleStore CreateTokenHandleStore()
        {
            return new TokenHandleStore(_dbFactory);
        }

        public IRefreshTokenStore CreateRefreshTokenStore()
        {
            return new RefreshTokenStore(_dbFactory);
        }

        public void ConfigureClients(IEnumerable<Client> clients)
        {
            using (var db = _dbFactory.Create())
            {
                if (!db.Clients.Any())
                {
                    foreach (var c in clients)
                    {
                        var e = c.ToEntity();
                        db.Clients.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        public void ConfigureScopes(IEnumerable<Scope> scopes)
        {
            using (var db = _dbFactory.Create())
            {
                if (!db.Scopes.Any())
                {
                    foreach (var s in scopes)
                    {
                        var e = s.ToEntity();
                        db.Scopes.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}
