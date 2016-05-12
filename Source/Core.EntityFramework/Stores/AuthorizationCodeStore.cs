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
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.EntityFramework.Entities;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace IdentityServer3.EntityFramework
{
    public class AuthorizationCodeStore : BaseTokenStore<AuthorizationCode>, IAuthorizationCodeStore
    {
        public AuthorizationCodeStore(IOperationalDbContext context, IScopeStore scopeStore, IClientStore clientStore)
            : base(context, TokenType.AuthorizationCode, scopeStore, clientStore)
        {
        }

        public override async Task StoreAsync(string key, AuthorizationCode code)
        {
            var efCode = new Entities.Token
            {
                Key = key,
                SubjectId = code.SubjectId,
                ClientId = code.ClientId,
                JsonCode = ConvertToJson(code),
                Expiry = DateTimeOffset.UtcNow.AddSeconds(code.Client.AuthorizationCodeLifetime),
                TokenType = this.tokenType
            };

            context.Tokens.Add(efCode);
            await context.SaveChangesAsync();
        }

        protected override async Task<AuthorizationCode> ConvertFromJsonAsync( string json )
        {
          var token = await base.ConvertFromJsonAsync(json);
          token.Client = await clientStore.FindClientByIdAsync(token.ClientId);
          token.RequestedScopes = await scopeStore.FindScopesAsync(token.RequestedScopes.Select(s => s.Name));
          return token;
        }
    }
}
