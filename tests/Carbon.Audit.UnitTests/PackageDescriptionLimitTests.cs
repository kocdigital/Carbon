using System.Net;
using System.Text.RegularExpressions;
using Xunit;

namespace Carbon.Audit.UnitTests;

// nuget.org rejects a push whose Description exceeds 4000 characters, and the publish workflow stops
// at the first rejection - leaving dependants published without their dependency. Catch it in CI instead.
public class PackageDescriptionLimitTests
{
    private const int NuGetDescriptionLimit = 4000;

    [Fact]
    public void Every_project_description_fits_nuget_org_limit()
    {
        var root = FindRepoRoot();
        var tooLong = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                     && !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Select(p => (Path: Path.GetRelativePath(root, p), Match: Regex.Match(File.ReadAllText(p), "<Description>(.*?)</Description>", RegexOptions.Singleline)))
            .Where(x => x.Match.Success)
            .Select(x => (x.Path, Length: WebUtility.HtmlDecode(x.Match.Groups[1].Value).Trim().Length))
            .Where(x => x.Length > NuGetDescriptionLimit)
            .Select(x => $"{x.Path}: {x.Length}")
            .ToList();

        Assert.True(tooLong.Count == 0, "Description over 4000 characters: " + string.Join(", ", tooLong));
    }

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "Carbon Framework.sln"))) return dir.FullName;
        throw new DirectoryNotFoundException($"Carbon Framework.sln not found above {AppContext.BaseDirectory}");
    }
}
