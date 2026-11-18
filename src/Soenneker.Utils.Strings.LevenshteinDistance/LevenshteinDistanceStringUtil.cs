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
    /// <param name="s1">The first string.</param>
    /// <param name="s2">The second string.</param>
    /// <returns>The minimum number of unit-cost insertions, deletions, and substitutions needed to transform one string into the other.</returns>
    [Pure]
    public static int ComputeDistance(string s1, string s2)
    {
        ReadOnlySpan<char> rows = s1;
        ReadOnlySpan<char> columns = s2;

        int prefixLength = rows.CommonPrefixLength(columns);
        rows = rows[prefixLength..];
        columns = columns[prefixLength..];
        while (!rows.IsEmpty && !columns.IsEmpty && rows[^1] == columns[^1])
        {
            rows = rows[..^1];
            columns = columns[..^1];
        }

        if (rows.IsEmpty)
            return columns.Length;
        if (columns.IsEmpty)
            return rows.Length;

        // The row buffers are proportional to the shorter input.
        if (columns.Length > rows.Length)
        {
            ReadOnlySpan<char> temp = rows;
            rows = columns;
            columns = temp;
        }

        if (columns.Length is >= 4 and <= 1024 && System.Text.Ascii.IsValid(columns))
        {
            return columns.Length <= 64
                ? ShortAsciiDistance(rows, columns)
                : BlockAsciiDistance(rows, columns);
        }

        return RowDistance(rows, columns);
    }

    private static int RowDistance(ReadOnlySpan<char> rows, ReadOnlySpan<char> columns)
    {
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

    // Extend the same recurrence across words. Addition and both shifts carry into the next word.
    private static int BlockAsciiDistance(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern)
    {
        int blocks = (pattern.Length + 63) / 64;
        int maskLength = 128 * blocks;
        ulong[] rented = ArrayPool<ulong>.Shared.Rent(maskLength);
        Span<ulong> masks = rented.AsSpan(0, maskLength);
        try
        {
            masks.Clear();
            for (int i = 0; i < pattern.Length; i++)
                masks[pattern[i] * blocks + (i >> 6)] |= 1UL << (i & 63);

            Span<ulong> positive = stackalloc ulong[blocks];
            Span<ulong> negative = stackalloc ulong[blocks];
            positive.Fill(ulong.MaxValue);
            negative.Clear();
            ulong last = 1UL << ((pattern.Length - 1) & 63);
            int distance = pattern.Length;
            foreach (char character in text)
            {
                ulong carry = 0, increaseCarry = 1, decreaseCarry = 0;
                ulong increase = 0, decrease = 0;
                int start = character * blocks;
                for (int block = 0; block < blocks; block++)
                {
                    ulong equal = character < 128 ? masks[start + block] : 0;
                    ulong p = positive[block];
                    ulong n = negative[block];
                    ulong vertical = equal | n;
                    UInt128 sum = (UInt128)(equal & p) + p + carry;
                    carry = (ulong)(sum >> 64);
                    ulong horizontal = (unchecked((ulong)sum) ^ p) | equal;
                    increase = n | ~(horizontal | p);
                    decrease = p & horizontal;
                    ulong shiftedIncrease = (increase << 1) | increaseCarry;
                    ulong shiftedDecrease = (decrease << 1) | decreaseCarry;
                    increaseCarry = increase >> 63;
                    decreaseCarry = decrease >> 63;
                    positive[block] = shiftedDecrease | ~(vertical | shiftedIncrease);
                    negative[block] = shiftedIncrease & vertical;
                }
                if ((increase & last) != 0)
                    distance++;
                else if ((decrease & last) != 0)
                    distance--;
            }
            return distance;
        }
        finally
        {
            ArrayPool<ulong>.Shared.Return(rented, clearArray: true);
        }
    }

    // Myers' bit-vector recurrence tracks one DP column in two words.
    // Each bit in a character's mask identifies one matching position in the shorter string.
    private static int ShortAsciiDistance(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern)
    {
        Span<ulong> matches = stackalloc ulong[128];
        matches.Clear();
        for (int i = 0; i < pattern.Length; i++)
            matches[pattern[i]] |= 1UL << i;

        ulong positive = ulong.MaxValue;
        ulong negative = 0;
        ulong last = 1UL << (pattern.Length - 1);
        int distance = pattern.Length;
        foreach (char character in text)
        {
            ulong equal = character < matches.Length ? matches[character] : 0;
            ulong vertical = equal | negative;
            ulong horizontal = (unchecked((equal & positive) + positive) ^ positive) | equal;
            ulong increase = negative | ~(horizontal | positive);
            ulong decrease = positive & horizontal;
            if ((increase & last) != 0)
                distance++;
            else if ((decrease & last) != 0)
                distance--;

            increase = (increase << 1) | 1;
            decrease <<= 1;
            positive = decrease | ~(vertical | increase);
            negative = increase & vertical;
        }
        return distance;
    }
}
