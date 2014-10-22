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
using Newtonsoft.Json;
using Thinktecture.IdentityServer.Core.EntityFramework.Serialization;
using Thinktecture.IdentityServer.Core.EntityFramework.Entities;
using System.Data.Entity;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public abstract class BaseTokenStore<T> where T : class
    {
        private readonly CoreDbContext _db;
        protected readonly TokenType TokenType;

        protected CoreDbContext Db
        {
            get { return _db; }
        }

        protected BaseTokenStore(CoreDbContext db, TokenType tokenType)
        {
            _db = db;
            TokenType = tokenType;
        }

        JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ClaimConverter());
            settings.Converters.Add(new ClaimsPrincipalConverter());
            settings.Converters.Add(new ClientConverter(new ClientStore(Db)));
            var svc = new ScopeStore(Db);
            var scopes = AsyncHelper.RunSync(async () => await svc.GetScopesAsync());
            settings.Converters.Add(new ScopeConverter(scopes.ToArray()));
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
            var db = _db;
            var token = await db.Tokens.FirstOrDefaultAsync(c => c.Key == key && c.TokenType == TokenType);
            if (token == null || token.Expiry < DateTime.UtcNow) return null;

            T value = ConvertFromJson(token.JsonCode);
            return value;
        }

        public async Task RemoveAsync(string key)
        {
            var db = _db;
            var code = await db.Tokens.FirstOrDefaultAsync(c => c.Key == key && c.TokenType == TokenType);

            if (code != null)
            {
                db.Tokens.Remove(code);
                await db.SaveChangesAsync();
            }
        }

        public abstract Task StoreAsync(string key, T value);
    }
}
