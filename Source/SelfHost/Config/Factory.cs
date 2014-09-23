using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.EntityFramework;
using Thinktecture.IdentityServer.Core.Services;

namespace SelfHost.Config
{
    class Factory
    {
        public static IdentityServerServiceFactory Configure(string connString)
        {
            var svcFactory = new ServiceFactory(new CoreDbContextFactoryViaConnectionString(connString));
            svcFactory.ConfigureClients(Clients.Get());
            svcFactory.ConfigureScopes(Scopes.Get());

            var factory = new IdentityServerServiceFactory();

            var userService = new Thinktecture.IdentityServer.Core.Services.InMemory.InMemoryUserService(Users.Get());
            factory.UserService = Registration.RegisterFactory<IUserService>(() => userService);

            factory.ScopeStore = Registration.RegisterFactory<IScopeStore>(svcFactory.CreateScopeStore);
            factory.ClientStore = Registration.RegisterFactory<IClientStore>(svcFactory.CreateClientStore);

            factory.AuthorizationCodeStore = Registration.RegisterFactory<IAuthorizationCodeStore>(svcFactory.CreateAuthorizationCodeStore);
            factory.TokenHandleStore = Registration.RegisterFactory<ITokenHandleStore>(svcFactory.CreateTokenHandleStore);
            factory.ConsentService = Registration.RegisterFactory<IConsentService>(svcFactory.CreateConsentService);
            factory.RefreshTokenStore = Registration.RegisterFactory<IRefreshTokenStore>(svcFactory.CreateRefreshTokenStore);

            // Remove expired tokens every 5 minutes
            ExpiredTokenCollector.Start(connString, 5);

            return factory;
        }
    }
}
