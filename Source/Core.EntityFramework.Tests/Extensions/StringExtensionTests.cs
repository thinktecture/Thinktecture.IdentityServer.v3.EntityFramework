using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace IdentityServer3.EntityFramework.Tests.Extensions
{ 
    public class StringExtensionTests
    {
        private readonly ITestOutputHelper _output;

        public StringExtensionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanStringifyAListOfScopes()
        {
            var scopes = new List<string>
            {
                "Scope 1",
                "Scope 2",
                "Scope 3"
            };

            var stringified = scopes.GetAsCommaSeparatedString();

            Assert.Equal("Scope 1,Scope 2,Scope 3", stringified);
        }

        [Fact]
        public void IsFasterThanOldImplementation()
        {
            const int iterations = 10*1000;

            var scopes = new List<string>();
            for (var i = 0; i < 50; i++)
            {
                scopes.Add(string.Format("Scope {0}", i));
            }

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (var i = 0; i < iterations; i++)
            {
                scopes.GetAsCommaSeparatedString();
            }
            stopwatch.Stop();

            var newImplTimeSpan = stopwatch.Elapsed;

            stopwatch.Reset();
            stopwatch.Start();
            for (var i = 0; i < iterations; i++)
            {
                OldStringifyScopes(scopes);
            }
            stopwatch.Stop();

            var oldImplTimeSpan = stopwatch.Elapsed;

            _output.WriteLine("Old implementation took: {0}ms; New implemenation took: {1}ms", oldImplTimeSpan, newImplTimeSpan);
            Assert.True(newImplTimeSpan < oldImplTimeSpan);
        }

        private string OldStringifyScopes(IEnumerable<string> scopes)
        {
            if (scopes == null || !scopes.Any())
            {
                return null;
            }

            return scopes.Aggregate((s1, s2) => s1 + "," + s2);
        }
    }
}