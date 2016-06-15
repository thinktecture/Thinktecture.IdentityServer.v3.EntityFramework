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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

namespace IdentityServer3.EntityFramework
{
    public class ConsentStore : IConsentStore
    {
        private readonly IOperationalDbContext context;

        public ConsentStore(IOperationalDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            
            this.context = context;
        }

        public async Task<IdentityServer3.Core.Models.Consent> LoadAsync(string subject, string client)
        {
            var found = await context.Consents.FindAsync(subject, client);
            if (found == null)
            {
                return null;
            }
                
            var result = new IdentityServer3.Core.Models.Consent
            {
                Subject = found.Subject,
                ClientId = found.ClientId,
                Scopes = ParseScopes(found.Scopes)
            };

            return result;
        }

        public async Task UpdateAsync(IdentityServer3.Core.Models.Consent consent)
        {
            var item = await context.Consents.FindAsync(consent.Subject, consent.ClientId);
            if (item == null)
            {
                item = new Entities.Consent 
                { 
                    Subject = consent.Subject, 
                    ClientId = consent.ClientId 
                };
                context.Consents.Add(item);
            }
                
            if (consent.Scopes == null || !consent.Scopes.Any())
            {
                context.Consents.Remove(item);
            }

            item.Scopes = consent.Scopes.GetAsCommaSeparatedString();

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<IdentityServer3.Core.Models.Consent>> LoadAllAsync(string subject)
        {
            var found = await context.Consents.Where(x => x.Subject == subject).ToArrayAsync();
            
            var results = found.Select(x=>new IdentityServer3.Core.Models.Consent{
                Subject = x.Subject, 
                ClientId = x.ClientId, 
                Scopes = ParseScopes(x.Scopes) 
            });

            return results.ToArray().AsEnumerable();
        }

        private IEnumerable<string> ParseScopes(string scopes)
        {
            if (scopes == null || String.IsNullOrWhiteSpace(scopes))
            {
                return Enumerable.Empty<string>();
            }

            return scopes.Split(',');
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var found = await context.Consents.FindAsync(subject, client);

            if (found != null)
            {
                context.Consents.Remove(found);
                await context.SaveChangesAsync();
            }
        }
    }
}
