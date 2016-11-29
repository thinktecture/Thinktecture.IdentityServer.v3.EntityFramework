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
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;
using System.Data.Entity;

namespace IdentityServer3.EntityFramework
{
    public class ClientConfigurationCorsPolicyService : ICorsPolicyService
    {
        private readonly IClientConfigurationDbContext context;

        public ClientConfigurationCorsPolicyService(IClientConfigurationDbContext ctx)
        {
            this.context = ctx;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var query =
                from client in context.Clients
                from allowed in client.AllowedCorsOrigins
                select allowed.Origin;
            var urls = await query.ToArrayAsync();

            var origins = urls.Select(x => x.GetOrigin()).Where(x => x != null).Distinct().ToList();

            return urls.ToList().Contains("*") || origins.Contains(origin, StringComparer.OrdinalIgnoreCase);
        }
    }
}
