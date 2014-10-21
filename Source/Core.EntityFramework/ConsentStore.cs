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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Extensions;
using Thinktecture.IdentityServer.Core.Services;
using System.Data.Entity;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ConsentStore : IConsentStore
    {
        private readonly CoreDbContext _db;

        public ConsentStore(CoreDbContext db)
        {
            _db = db;
        }

        public async Task<bool> RequiresConsentAsync(string client, string subject, IEnumerable<string> scopes)
        {
            var orderedScopes = GetOrderedScopes(scopes);

            var db = _db;
            {
                var exists = await db.Consents.AnyAsync(c => c.ClientId == client &&
                                                c.Scopes == orderedScopes &&
                                                c.Subject == subject);

                return !exists;
            }
        }

        private static string GetOrderedScopes(IEnumerable<string> scopes)
        {
            return string.Join(" ", scopes.OrderBy(s => s).ToArray());
        }

        public async Task UpdateConsentAsync(string client, string subject, IEnumerable<string> scopes)
        {
            var db = _db;
            {
                var consent = await db.Consents.FindAsync(subject, client);

                if (scopes.Any())
                {
                    if (consent == null)
                    {
                        consent = new Entities.Consent
                        {
                            ClientId = client,
                            Subject = subject,
                        };
                        db.Consents.Add(consent);
                    }

                    consent.Scopes = GetOrderedScopes(scopes);
                }
                else if (consent != null)
                {
                    db.Consents.Remove(consent);
                }

                await db.SaveChangesAsync();
            }
        }
    }
}
