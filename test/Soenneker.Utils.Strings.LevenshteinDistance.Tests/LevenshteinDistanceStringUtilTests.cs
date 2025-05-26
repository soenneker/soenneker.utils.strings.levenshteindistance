using AwesomeAssertions;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Utils.Strings.LevenshteinDistance.Tests;

[Collection("Collection")]
public class LevenshteinDistanceStringUtilTests : FixturedUnitTest
{

    public LevenshteinDistanceStringUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Theory]
    [InlineData("", "", 1)] // Two empty strings are identical
    [InlineData("abc", "", 0)] // Completely different (non-overlapping) strings
    [InlineData("", "xyz", 0)] // Completely different (non-overlapping) strings
    [InlineData("kitten", "sitting", 0.5714)] // Levenshtein Distance = 3, max length = 7
    [InlineData("kitten", "kitten", 1)] // Identical strings
    [InlineData("abc", "def", 0)] // Completely different strings
    [InlineData("abcdef", "abc", 0.5)] // Levenshtein Distance = 3, max length = 6
    [InlineData("abc", "abcd", 0.75)] // Levenshtein Distance = 1, max length = 4
    [InlineData("this is sitting on a porch", "this is sitting", 0.576)] // Adjusted based on edits
    [InlineData("the cat sat on a hat", "sad on a hat", 0.55)] // Adjusted for deletions
    [InlineData("this is a test", "this is another test", 0.7)] // Adjusted for substitutions and insertions
    public void CalculateSimilarity_Returns_Correct_Similarity_Score(string str1, string str2, double expectedScore)
    {
        double similarityScore = LevenshteinDistanceStringUtil.Calculate(str1, str2);

        similarityScore.Should().BeApproximately(expectedScore, 0.001);
    }
}
