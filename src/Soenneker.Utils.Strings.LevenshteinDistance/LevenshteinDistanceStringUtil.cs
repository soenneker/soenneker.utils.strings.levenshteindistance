using System.Diagnostics.Contracts;
using System;
using System.Buffers;

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
        ReadOnlySpan<char> rows = s1;
        ReadOnlySpan<char> columns = s2;

        // The row buffers are proportional to the shorter input.
        if (columns.Length > rows.Length)
        {
            ReadOnlySpan<char> temp = rows;
            rows = columns;
            columns = temp;
        }

        int width = columns.Length + 1;
        int[]? rented = null;
        Span<int> storage = width <= 256
            ? stackalloc int[width * 2]
            : (rented = ArrayPool<int>.Shared.Rent(width * 2)).AsSpan(0, width * 2);

        try
        {
            Span<int> previous = storage[..width];
            Span<int> current = storage[width..];

            for (var j = 0; j < width; j++)
                previous[j] = j;

            for (var i = 1; i <= rows.Length; i++)
            {
                current[0] = i;

                for (var j = 1; j < width; j++)
                {
                    int cost = rows[i - 1] == columns[j - 1] ? 0 : 1;
                    current[j] = Math.Min(previous[j] + 1, Math.Min(current[j - 1] + 1, previous[j - 1] + cost));
                }

                Span<int> swap = previous;
                previous = current;
                current = swap;
            }

            return previous[^1];
        }
        finally
        {
            if (rented is not null)
                ArrayPool<int>.Shared.Return(rented);
        }
    }
}
