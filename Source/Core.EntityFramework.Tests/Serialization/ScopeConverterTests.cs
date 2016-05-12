using System;
using System.Linq;
using System.Security.Claims;
using IdentityServer3.Core;
using Newtonsoft.Json;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework.Serialization;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Xunit;

namespace IdentityServer3.EntityFramework.Tests.Serialization
{
    public class ScopeConverterTests
    {
        [Fact]
        public void CanSerializeAndDeserializeAScope()
        {
            var s1 = new Scope
            {
                Name = "email",
                Required = true,
                Type = ScopeType.Identity,
                Emphasize = true,
                DisplayName = "email foo",
                Description = "desc foo",
                Claims = new List<ScopeClaim> { 
                    new ScopeClaim{Name = "email", Description = "email"}
                }
            };
            var s2 = new Scope
            {
                Name = "read",
                Required = true,
                Type = ScopeType.Resource,
                Emphasize = true,
                DisplayName = "foo",
                Description = "desc",
            };
            var converter = new ScopeConverter();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);
            var json = JsonConvert.SerializeObject(s1, settings);

            var result = JsonConvert.DeserializeObject<Scope>(json, settings);
            Assert.IsType<Scope>(result);
            Assert.Equal(s1.Name, result.Name);
        }
    }
}
