using System.IO;
using System.Text.RegularExpressions;

namespace Shop.Tests.Boundaries
{
    /// <summary>
    /// A payload fixture shows what the other system sends only if that system's documentation says so.
    /// One written by the same hand as the parser proves only that the two agree.
    /// </summary>
    public class FixtureProvenanceTests
    {
        private const string ProviderDocs = "docs/provider/examplepay-webhooks.md";

        private const string Fixtures = "tests/Shop.Tests/Payments/Fixtures";

        [Fact]
        public void Fixtures_ShouldBeExamplesFromTheProviderDocs()
        {
            // Arrange:
            // Every fixture must be one of the JSON examples on the provider's page, unchanged
            var root = GetRepositoryRoot();

            var examples = GetJsonExamples(File.ReadAllText(Path.Combine(root, ProviderDocs)));

            // Act
            var invented = Directory
                .GetFiles(Path.Combine(root, Fixtures), "*.json")
                .Where(_ => !examples.Contains(Normalize(File.ReadAllText(_))))
                .Select(Path.GetFileName);

            // Assert
            invented.ShouldBeEmpty($"Not an example from {ProviderDocs}. Copy payloads from the provider's documentation. Never write one to fit the parser.");
        }

        private static HashSet<string> GetJsonExamples(string markdown)
        {
            return Regex
                .Matches(Normalize(markdown), @"```json\n(.*?)```", RegexOptions.Singleline)
                .Select(_ => Normalize(_.Groups[1].Value))
                .ToHashSet();
        }

        private static string Normalize(string text)
        {
            return text.ReplaceLineEndings("\n").Trim();
        }

        private static string GetRepositoryRoot()
        {
            for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
            {
                if (File.Exists(Path.Combine(directory.FullName, "AgentsCanWriteTests.slnx")))
                {
                    return directory.FullName;
                }
            }

            throw new InvalidOperationException("Repository root not found.");
        }
    }
}