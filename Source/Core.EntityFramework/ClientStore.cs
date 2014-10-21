/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ClientStore : IClientStore
    {
        private readonly CoreDbContext _db;

        public ClientStore(CoreDbContext db)
        {
            _db = db;
        }

        public Task<Models.Client> FindClientByIdAsync(string clientId)
        {
            var db = _db;
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
