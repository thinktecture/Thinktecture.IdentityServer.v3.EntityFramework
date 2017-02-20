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
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using IdentityServer3.EntityFramework.Entities;
using IdentityServer3.Core.Services;

namespace IdentityServer3.EntityFramework
{
    public class ScopeStore : IScopeStore
    {
        private readonly IScopeConfigurationDbContext context;
        private readonly EntityFrameworkServiceOptions options;

        public ScopeStore(IScopeConfigurationDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.context = context;
        }

        public ScopeStore(EntityFrameworkServiceOptions options, IScopeConfigurationDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.options = options;
            this.context = context;
        }

        public async Task<IEnumerable<IdentityServer3.Core.Models.Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            var scopes = context.Scopes
                .AsNoTracking()
                .Include(x => x.ScopeClaims)
                .Include(x => x.ScopeSecrets);
                
            if (scopeNames != null && scopeNames.Any())
            {
                scopes = scopes
                    .Where(x => scopeNames.Contains(x.Name));
            }

            Scope[] list = null;
            if (options != null && options.SynchronousReads)
            {
                list = scopes.ToArray();
            }
            else
            {
                list = await scopes.ToArrayAsync();
            }

            return list.Select(x => x.ToModel());
        }

        public async Task<IEnumerable<IdentityServer3.Core.Models.Scope>> GetScopesAsync(bool publicOnly = true)
        {
            var scopes = context.Scopes
                .AsNoTracking()
                .Include(x => x.ScopeClaims)
                .Include(x => x.ScopeSecrets);
                
            if (publicOnly)
            {
                scopes = from s in scopes
                            where s.ShowInDiscoveryDocument == true
                            select s;
            }

            Scope[] list = null;
            if (options != null && options.SynchronousReads)
            {
                list = scopes.ToArray();
            }
            else
            {
                list = await scopes.ToArrayAsync();
            }
            return list.Select(x => x.ToModel());
        }
    }
}
