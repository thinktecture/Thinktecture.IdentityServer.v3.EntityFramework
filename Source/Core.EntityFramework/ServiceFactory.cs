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
using System.Data.Entity;
using System.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ServiceFactory
    {
        private readonly CoreDbContext _db;
        public ServiceFactory(CoreDbContext connectionString)
        {
            _db = connectionString;
            Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDbContext>());
        }

        public IClientStore CreateClientStore()
        {
            return new ClientStore(_db);
        }

        public IScopeStore CreateScopeStore()
        {
            return new ScopeStore(_db);
        }

        public IConsentStore CreateConsentStore()
        {
            return new ConsentStore(_db);
        }

        public IAuthorizationCodeStore CreateAuthorizationCodeStore()
        {
            return new AuthorizationCodeStore(_db);
        }

        public ITokenHandleStore CreateTokenHandleStore()
        {
            return new TokenHandleStore(_db);
        }

        public IRefreshTokenStore CreateRefreshTokenStore()
        {
            return new RefreshTokenStore(_db);
        }

        public void ConfigureClients(IEnumerable<Client> clients)
        {
            var db = _db;
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
            var db = _db;
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
