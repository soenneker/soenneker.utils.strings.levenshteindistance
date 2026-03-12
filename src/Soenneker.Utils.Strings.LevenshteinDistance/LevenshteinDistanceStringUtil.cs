using System.Diagnostics.Contracts;
using System;

namespace Soenneker.Utils.Strings.LevenshteinDistance;

/// <summary>
/// A utility library for comparing strings via the Levenshtein Distance algorithm
/// </summary>
public static class LevenshteinDistanceStringUtil
{
    /// <summary>
    /// Calculates the similarity percentage between two strings via Levenshtein Distance.
    /// </summary>
    /// <param name="s1">The first string.</param>
    /// <param name="s2">The second string.</param>
    /// <returns>The similarity percentage between the two strings.</returns>
    [Pure]
    public static double CalculatePercentage(string s1, string s2)
    {
        double similarity = Calculate(s1, s2);
        double percentageMatch = similarity * 100;

        return percentageMatch;
    }

    /// <summary>
    /// Calculates the similarity score between two strings using the Levenshtein Distance.
    /// </summary>
    /// <param name="s1">The first string.</param>
    /// <param name="s2">The second string.</param>
    /// <returns>The similarity score between the two strings.</returns>
    [Pure]
    public static double Calculate(string s1, string s2)
    {
        if (s1.Length == 0 && s2.Length == 0)
            return 1.0;

        int distance = ComputeDistance(s1, s2);
        int maxLength = Math.Max(s1.Length, s2.Length);
        double similarity = 1.0 - (double)distance / maxLength;

        return similarity;
    }

    /// <summary>
    /// Computes the Levenshtein Distance between two strings.
    /// </summary>
    [Pure]
    public static int ComputeDistance(string s1, string s2)
    {
        int len1 = s1.Length;
        int len2 = s2.Length;

        var dp = new int[len1 + 1, len2 + 1];

        for (int i = 0; i <= len1; i++)
            dp[i, 0] = i;

        for (int j = 0; j <= len2; j++)
            dp[0, j] = j;

        for (int i = 1; i <= len1; i++)
        {
            for (int j = 1; j <= len2; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;

                dp[i, j] = Math.Min(
                    dp[i - 1, j] + 1,                    // Deletion
                    Math.Min(dp[i, j - 1] + 1,          // Insertion
                             dp[i - 1, j - 1] + cost)); // Substitution
            }
        }

        return dp[len1, len2];
    }
}
