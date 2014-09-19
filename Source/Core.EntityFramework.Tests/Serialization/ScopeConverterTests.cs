using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using Thinktecture.IdentityServer.Core;
using Newtonsoft.Json;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.EntityFramework.Serialization;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core.Services.InMemory;
using System.Threading.Tasks;
using System.Threading;

namespace Thinktecture.IdentityServer.v3.EntityFramework.Tests.Serialization
{
    [TestClass]
    public class ScopeConverterTests
    {
        [TestMethod]
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
                Claims = new ScopeClaim[] { 
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
            var converter = new ScopeConverter(new Scope[] { s1, s2 });

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);
            var json = JsonConvert.SerializeObject(s1, settings);

            var result = JsonConvert.DeserializeObject<Scope>(json, settings);
            Assert.AreSame(s1, result);
        }
    }
}
