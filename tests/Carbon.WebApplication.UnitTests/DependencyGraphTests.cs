using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    /// <summary>
    /// Carbon.WebApplication must restore alongside MassTransit 7 and 8 alike, so nothing in its
    /// dependency graph may bring a message transport. This reads the resolved graph of this test
    /// project (which references only Carbon.WebApplication and test tooling), so it also catches
    /// an unused PackageReference that assembly metadata would not show.
    /// </summary>
    public class DependencyGraphTests
    {
        private static readonly string[] ForbiddenPrefixes =
        {
            "MassTransit", "Carbon.MassTransit", "RabbitMQ.Client", "GreenPipes", "Carbon.Audit.MassTransit"
        };

        [Fact]
        public void Carbon_WebApplication_graph_contains_no_message_transport()
        {
            var depsFile = Path.Combine(AppContext.BaseDirectory, "Carbon.WebApplication.UnitTests.deps.json");
            Assert.True(File.Exists(depsFile), $"deps file not found: {depsFile}");

            using var doc = JsonDocument.Parse(File.ReadAllText(depsFile));
            var libraries = doc.RootElement.GetProperty("libraries").EnumerateObject()
                .Select(p => p.Name.Split('/')[0])
                .ToList();

            Assert.Contains("Carbon.Audit", libraries);
            var forbidden = libraries
                .Where(name => ForbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            Assert.Empty(forbidden);
        }
    }
}
