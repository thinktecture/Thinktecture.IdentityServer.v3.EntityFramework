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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using IdentityServer3.EntityFramework.Entities;
using IdentityServer3.EntityFramework.Serialization;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace IdentityServer3.EntityFramework
{
    public abstract class BaseTokenStore<T> where T : class
    {
        protected readonly IOperationalDbContext context;
        protected readonly TokenType tokenType;
        protected readonly IScopeStore scopeStore;
        protected readonly IClientStore clientStore;
        protected readonly EntityFrameworkServiceOptions options;

        protected BaseTokenStore(IOperationalDbContext context, TokenType tokenType, IScopeStore scopeStore, IClientStore clientStore)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (scopeStore == null) throw new ArgumentNullException("scopeStore");
            if (clientStore == null) throw new ArgumentNullException("clientStore");
            
            this.context = context;
            this.tokenType = tokenType;
            this.scopeStore = scopeStore;
            this.clientStore = clientStore;
        }

        protected BaseTokenStore(EntityFrameworkServiceOptions options, IOperationalDbContext context, TokenType tokenType, IScopeStore scopeStore, IClientStore clientStore)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (scopeStore == null) throw new ArgumentNullException("scopeStore");
            if (clientStore == null) throw new ArgumentNullException("clientStore");

            this.options = options;
            this.context = context;
            this.tokenType = tokenType;
            this.scopeStore = scopeStore;
            this.clientStore = clientStore;
        }

        JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ClaimConverter());
            settings.Converters.Add(new ClaimsPrincipalConverter());
            settings.Converters.Add(new ClientConverter(clientStore));
            settings.Converters.Add(new ScopeConverter(scopeStore));
            return settings;
        }

        protected string ConvertToJson(T value)
        {
            return JsonConvert.SerializeObject(value, GetJsonSerializerSettings());
        }

        protected T ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSerializerSettings());
        }

        public async Task<T> GetAsync(string key)
        {
            Entities.Token token = null;
            if (options != null && options.SynchronousReads)
            {
                token = context.Tokens.Find(key, tokenType);
            }
            else
            {
                token = await context.Tokens.FindAsync(key, tokenType);
            }

            if (token == null)
            {
                return null;
            }

            return ConvertFromJson(token.JsonCode);
        }

        public async Task RemoveAsync(string key)
        {
            Entities.Token token = null;
            if (options != null && options.SynchronousReads)
            {
                token = context.Tokens.Find(key, tokenType);
            }
            else
            {
                token = await context.Tokens.FindAsync(key, tokenType);
            }

            if (token != null)
            {
                context.Tokens.Remove(token);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            Entities.Token[] tokens = null;
            if (options != null && options.SynchronousReads)
            {
                tokens = context.Tokens.Where(x =>
                    x.SubjectId == subject &&
                    x.TokenType == tokenType).ToArray();
            }
            else
            {
                tokens = await context.Tokens.Where(x => 
                    x.SubjectId == subject &&
                    x.TokenType == tokenType).ToArrayAsync();
            }

            var results = tokens.Select(x=>ConvertFromJson(x.JsonCode)).ToArray();
            return results.Cast<ITokenMetadata>();
        }
        
        public async Task RevokeAsync(string subject, string client)
        {
            Entities.Token[] tokens = null;
            if (options != null && options.SynchronousReads)
            {
                tokens = context.Tokens.Where(x =>
                    x.SubjectId == subject &&
                    x.ClientId == client &&
                    x.TokenType == tokenType).ToArray();
            }
            else
            {
                tokens = await context.Tokens.Where(x => 
                    x.SubjectId == subject && 
                    x.ClientId == client && 
                    x.TokenType == tokenType).ToArrayAsync();
            }

            context.Tokens.RemoveRange(tokens);
            await context.SaveChangesAsync();
        }

        public abstract Task StoreAsync(string key, T value);
    }
}
