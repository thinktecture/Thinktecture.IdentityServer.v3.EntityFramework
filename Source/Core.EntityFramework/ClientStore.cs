using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ClientStore : IClientStore
    {
        private readonly CoreDbContextFactoryBase _dbFactory;

        public ClientStore(CoreDbContextFactoryBase dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public Task<Models.Client> FindClientByIdAsync(string clientId)
        {
            using (var db = _dbFactory.Create())
            {
                var client = db.Clients
                    .Include("RedirectUris")
                    .Include("ScopeRestrictions")
                    .SingleOrDefault(x => x.ClientId == clientId);

                Models.Client model = client.ToModel();
                return Task.FromResult(model);
            }
        }
    }
}
