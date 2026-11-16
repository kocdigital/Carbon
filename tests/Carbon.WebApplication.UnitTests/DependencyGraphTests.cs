using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    /// <summary>
    /// Carbon.WebApplication reaches MassTransit only through Carbon.Audit, which is built against
    /// MassTransit 7. This reads the resolved graph of this test project so that MassTransit 8
    /// cannot re-enter it unnoticed, whichever package brings it.
    /// </summary>
    public class DependencyGraphTests
    {
        [Fact]
        public void Carbon_WebApplication_graph_stays_on_MassTransit_7()
        {
            var depsFile = Path.Combine(AppContext.BaseDirectory, "Carbon.WebApplication.UnitTests.deps.json");
            Assert.True(File.Exists(depsFile), $"deps file not found: {depsFile}");

            using var doc = JsonDocument.Parse(File.ReadAllText(depsFile));
            var libraries = doc.RootElement.GetProperty("libraries").EnumerateObject()
                .Select(p => p.Name.Split('/'))
                .ToDictionary(p => p[0], p => p[1], StringComparer.OrdinalIgnoreCase);

            Assert.True(libraries.TryGetValue("MassTransit", out var massTransit), "MassTransit not in graph");
            Assert.StartsWith("7.", massTransit);
            Assert.True(libraries.TryGetValue("Carbon.MassTransit", out var carbonMassTransit), "Carbon.MassTransit not in graph");
            Assert.StartsWith("3.", carbonMassTransit);
            Assert.False(libraries.ContainsKey("MassTransit.Abstractions"), "MassTransit 8 (MassTransit.Abstractions) in graph");
            if (libraries.TryGetValue("RabbitMQ.Client", out var rabbit))
                Assert.StartsWith("6.", rabbit);
        }
    }
}
