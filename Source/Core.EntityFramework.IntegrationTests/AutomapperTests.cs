using AutoMapper;
using Xunit;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework.Entities;
using System.Collections.Generic;

namespace Core.EntityFramework.IntegrationTests
{
    public class AutomapperTests
    {
        [Fact]
        public void AutomapperConfigurationIsValid()
        {
            IdentityServer3.Core.Models.Scope s = new IdentityServer3.Core.Models.Scope()
            {
            };
            var e = s.ToEntity();

            IdentityServer3.Core.Models.Client c = new IdentityServer3.Core.Models.Client()
            {
            };
            var e2 = c.ToEntity();

            IdentityServer3.EntityFramework.Entities.Scope s2 = new IdentityServer3.EntityFramework.Entities.Scope()
            {
                ScopeClaims = new HashSet<IdentityServer3.EntityFramework.Entities.ScopeClaim>(),
                ScopeSecrets = new HashSet<IdentityServer3.EntityFramework.Entities.ScopeSecret>(),
            };
            var m = s2.ToModel();

            IdentityServer3.EntityFramework.Entities.EntitiesMap.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
            IdentityServer3.Core.Models.EntitiesMap.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}