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
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Connect.Models;
using Thinktecture.IdentityServer.Core.Services;
using System.Data.Entity;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class TokenHandleStore : BaseTokenStore<Token>, ITokenHandleStore
    {
        public TokenHandleStore(CoreDbContext db)
            : base(db, Entities.TokenType.TokenHandle)
        {
        }

        public override Task StoreAsync(string key, Token value)
        {
            var db = Db;
            {
                var efToken = new Entities.Token
                {
                    Key = key,
                    JsonCode = ConvertToJson(value),
                    Expiry = DateTime.UtcNow.AddSeconds(value.Lifetime),
                    TokenType = this.TokenType
                };

                db.Tokens.Add(efToken);
                return db.SaveChangesAsync();
            }

        }
    }
}
