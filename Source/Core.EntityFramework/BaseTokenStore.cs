using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Thinktecture.IdentityServer.Core.EntityFramework.Serialization;
using Thinktecture.IdentityServer.Core.EntityFramework.Entities;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public abstract class BaseTokenStore<T> where T : class
    {
        protected readonly TokenType TokenType;

        private readonly CoreDbContextFactoryBase _dbFactory;

        public CoreDbContextFactoryBase DbFactory
        {
            get { return _dbFactory; }
        }

        protected BaseTokenStore(CoreDbContextFactoryBase dbFactory, TokenType tokenType)
        {
            _dbFactory = dbFactory;
            TokenType = tokenType;
        }

        JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ClaimConverter());
            settings.Converters.Add(new ClaimsPrincipalConverter());
            settings.Converters.Add(new ClientConverter(new ClientStore(_dbFactory)));
            var svc = new ScopeStore(_dbFactory);
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

        public Task<T> GetAsync(string key)
        {
            using (var db = _dbFactory.Create())
            {
                var token = db.Tokens.FirstOrDefault(c => c.Key == key && c.TokenType == TokenType);
                if (token == null || token.Expiry < DateTime.UtcNow) return Task.FromResult<T>(null);

                T value = ConvertFromJson(token.JsonCode);
                return Task.FromResult(value);
            }
        }

        public Task RemoveAsync(string key)
        {
            using (var db = _dbFactory.Create())
            {
                var code = db.Tokens.FirstOrDefault(c => c.Key == key && c.TokenType == TokenType);

                if (code != null)
                {
                    db.Tokens.Remove(code);
                    db.SaveChanges();
                }
            }

            return Task.FromResult(0);
        }

        public abstract Task StoreAsync(string key, T value);
    }
}
