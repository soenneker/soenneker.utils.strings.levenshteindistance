using AwesomeAssertions;
using Soenneker.Tests.HostedUnit;


namespace Soenneker.Utils.Strings.LevenshteinDistance.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class LevenshteinDistanceStringUtilTests : HostedUnitTest
{

    public LevenshteinDistanceStringUtilTests(Host host) : base(host)
    {
    }

    [Test]
    [Arguments(63)]
    [Arguments(64)]
    [Arguments(65)]
    [Arguments(128)]
    [Arguments(129)]
    [Arguments(1024)]
    [Arguments(1025)]
    public void Distance_counts_edits_across_word_boundaries(int length)
    {
        string middle = new string('a', length - 2);
        string first = "x" + middle + "y";
        string second = "λ" + middle.Insert(middle.Length / 2, "!") + "z";

        LevenshteinDistanceStringUtil.ComputeDistance(first, second).Should().Be(3);
        LevenshteinDistanceStringUtil.ComputeDistance(second, first).Should().Be(3);
    }

    [Test]
    [Arguments("", "", 1)] // Two empty strings are identical
    [Arguments("abc", "", 0)] // Completely different (non-overlapping) strings
    [Arguments("", "xyz", 0)] // Completely different (non-overlapping) strings
    [Arguments("kitten", "sitting", 0.5714)] // Levenshtein Distance = 3, max length = 7
    [Arguments("kitten", "kitten", 1)] // Identical strings
    [Arguments("abc", "def", 0)] // Completely different strings
    [Arguments("abcdef", "abc", 0.5)] // Levenshtein Distance = 3, max length = 6
    [Arguments("abc", "abcd", 0.75)] // Levenshtein Distance = 1, max length = 4
    [Arguments("this is sitting on a porch", "this is sitting", 0.576)] // Adjusted based on edits
    [Arguments("the cat sat on a hat", "sad on a hat", 0.55)] // Adjusted for deletions
    [Arguments("this is a test", "this is another test", 0.7)] // Adjusted for substitutions and insertions
    public void CalculateSimilarity_Returns_Correct_Similarity_Score(string str1, string str2, double expectedScore)
    {
        double similarityScore = LevenshteinDistanceStringUtil.Calculate(str1, str2);

        similarityScore.Should().BeApproximately(expectedScore, 0.001);
    }
}

