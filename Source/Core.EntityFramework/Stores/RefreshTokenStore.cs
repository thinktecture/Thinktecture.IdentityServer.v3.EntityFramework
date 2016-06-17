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
using IdentityServer3.EntityFramework.Entities;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace IdentityServer3.EntityFramework
{
    public class RefreshTokenStore : BaseTokenStore<RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(IOperationalDbContext context, IScopeStore scopeStore, IClientStore clientStore)
            : base(context, TokenType.RefreshToken, scopeStore, clientStore)
        {
        }

        public override async Task StoreAsync(string key, RefreshToken value)
        {
            var token = await context.Tokens.FindAsync(key, tokenType);
            if (token == null)
            {
                token = new Entities.Token
                {
                    Key = key,
                    SubjectId = value.SubjectId,
                    ClientId = value.ClientId,
                    TokenType = tokenType
                };
                context.Tokens.Add(token);
            }

            token.JsonCode = ConvertToJson(value);
            token.Expiry = value.CreationTime.AddSeconds(value.LifeTime);

            await context.SaveChangesAsync();
        }
    }
}
