using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ScopeStore : IScopeStore
    {
        private readonly CoreDbContextFactoryBase _dbFactory;

        public ScopeStore(CoreDbContextFactoryBase dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public Task<IEnumerable<Models.Scope>> GetScopesAsync()
        {
            using (var db = _dbFactory.Create())
            {
                var scopes = db.Scopes
                    .Include("ScopeClaims");
                
                var models = scopes.ToList().Select(x => x.ToModel());

                return Task.FromResult(models);
            }
        }
    }
}
