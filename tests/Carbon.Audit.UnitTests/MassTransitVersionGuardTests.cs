using System.Text.RegularExpressions;
using Xunit;

namespace Carbon.Audit.UnitTests;

public class MassTransitVersionGuardTests
{
    [Fact]
    public void Guard_rejects_MassTransit_8_with_an_actionable_message()
    {
        var ex = Assert.Throws<InvalidOperationException>(
            () => ServiceCollectionExtensions.EnsureSupportedMassTransit(new Version(8, 5, 7, 0)));

        Assert.Contains("MassTransit 8.5.7", ex.Message);
        Assert.Contains("Carbon.Audit 2.0.0", ex.Message);
        Assert.Contains("CarbonAudit:Enabled", ex.Message);
    }

    [Theory]
    [InlineData("7.3.1.0")]
    [InlineData(null)]
    public void Guard_accepts_MassTransit_7_or_an_unknown_version(string? version)
    {
        var ex = Record.Exception(
            () => ServiceCollectionExtensions.EnsureSupportedMassTransit(version == null ? null : new Version(version)));

        Assert.Null(ex);
    }

    // The resolved graph inside this repository stays on MassTransit 7 even if the upper bounds are
    // dropped, so pin the declared ranges themselves: they are what keeps consumers off MassTransit 8.
    [Theory]
    [InlineData("Carbon.MassTransit", "[3.11.0,4.0.0)")]
    [InlineData("MassTransit", "[7.3.1,8.0.0)")]
    public void Carbon_Audit_declares_bounded_MassTransit_7_ranges(string package, string range)
    {
        var csproj = FindRepoFile(Path.Combine("Carbon.Audit", "Carbon.Audit.csproj"));
        var text = File.ReadAllText(csproj);

        var match = Regex.Match(text, $"<PackageReference Include=\"{Regex.Escape(package)}\" Version=\"([^\"]+)\"");
        Assert.True(match.Success, $"{package} reference not found in {csproj}");
        Assert.Equal(range, match.Groups[1].Value);
    }

    private static string FindRepoFile(string relative)
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, relative);
            if (File.Exists(candidate)) return candidate;
        }
        throw new FileNotFoundException($"{relative} not found above {AppContext.BaseDirectory}");
    }
}
