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

using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.EntityFramework.Entities;
using IdentityServer3.Core.Services;

namespace IdentityServer3.EntityFramework
{
    public class ClientStore : IClientStore
    {
        private readonly IClientConfigurationDbContext context;
        private readonly EntityFrameworkServiceOptions options;

        public ClientStore(IClientConfigurationDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.context = context;
        }

        public ClientStore(EntityFrameworkServiceOptions options, IClientConfigurationDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.options = options;
            this.context = context;
        }

        public async Task<IdentityServer3.Core.Models.Client> FindClientByIdAsync(string clientId)
        {
            var query = context.Clients
                    .AsNoTracking()
                    .Include(x => x.ClientSecrets)
                    .Include(x => x.RedirectUris)
                    .Include(x => x.PostLogoutRedirectUris)
                    .Include(x => x.AllowedScopes)
                    .Include(x => x.IdentityProviderRestrictions)
                    .Include(x => x.Claims)
                    .Include(x => x.AllowedCustomGrantTypes)
                    .Include(x => x.AllowedCorsOrigins);

            Client client = null;
            if (options != null && options.SynchronousReads)
            {
                client = query.SingleOrDefault(x => x.ClientId == clientId && x.Enabled);
            }
            else
            {
                client = await query.SingleOrDefaultAsync(x => x.ClientId == clientId && x.Enabled);
            }

            IdentityServer3.Core.Models.Client model = client.ToModel();
            return model;    
        }
    }
}
