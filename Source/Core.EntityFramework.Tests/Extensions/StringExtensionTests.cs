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
        private const int Iterations = 10 * 1000;

        private readonly ITestOutputHelper _output;
        private readonly List<string> _scopes = new List<string>();

        public StringExtensionTests(ITestOutputHelper output)
        {
            _output = output;

            for (var i = 0; i < 50; i++)
            {
                _scopes.Add(string.Format("Scope {0}", i));
            }
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
            var oldImplTimeSpan = GetExecutionDuration(OldStringifyScopes);
            var newImplTimeSpan = GetExecutionDuration(StringExtensions.GetAsCommaSeparatedString);

            _output.WriteLine("Old implementation took: {0}ms", oldImplTimeSpan);
            _output.WriteLine("New implemenation took: {0}ms", newImplTimeSpan);

            Assert.True(newImplTimeSpan < oldImplTimeSpan);
        }

        [Fact]
        public void UsesLessMemoryThanOldImplementation()
        {
            var oldUsage = GetMemoryUsage(OldStringifyScopes);
            var newUsage = GetMemoryUsage(StringExtensions.GetAsCommaSeparatedString);

            _output.WriteLine("Old implementation used {0} bytes", oldUsage);
            _output.WriteLine("New implemenation used {0} byes", newUsage);

            Assert.True(newUsage < oldUsage);
        }

        private TimeSpan GetExecutionDuration(Func<IEnumerable<string>, string> func)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (var i = 0; i < Iterations; i++)
            {
                func(_scopes);
            }
            stopwatch.Stop();

            return stopwatch.Elapsed;
        }

        private long GetMemoryUsage(Func<IEnumerable<string>, string> func)
        {
            var currentMemory = GC.GetTotalMemory(true);

            for (var i = 0; i < Iterations; i++)
            {
                func(_scopes);
            }

            var newMemory = GC.GetTotalMemory(false);

            return newMemory - currentMemory;
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