[![](https://img.shields.io/nuget/v/soenneker.utils.strings.levenshteindistance.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.strings.levenshteindistance/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.strings.levenshteindistance/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.strings.levenshteindistance/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.strings.levenshteindistance.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.strings.levenshteindistance/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.strings.levenshteindistance/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.strings.levenshteindistance/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Strings.LevenshteinDistance
Levenshtein edit distance with normalized and percentage similarity results.

## Installation

```bash
dotnet add package Soenneker.Utils.Strings.LevenshteinDistance
```

## Usage

```csharp
using Soenneker.Utils.Strings.LevenshteinDistance;

var text1 = "kitten";
var text2 = "sitting";

int distance = LevenshteinDistanceStringUtil.ComputeDistance(text1, text2);
double score = LevenshteinDistanceStringUtil.Calculate(text1, text2);
double percentage = LevenshteinDistanceStringUtil.CalculatePercentage(text1, text2);

// distance == 3
// score is approximately 0.5714
// percentage is approximately 57.14
```

`ComputeDistance` returns the minimum number of insertions, deletions, and substitutions needed to transform one input into the other. Each edit costs `1`.

`Calculate` normalizes the distance to a `0`–`1` similarity score:

```text
1 - distance / length of the longer input
```

`CalculatePercentage` multiplies that score by 100. Two empty strings return `1` (or `100%`), while an empty string compared with a non-empty string returns `0`.

## Comparison rules and cost

- Comparison is case-sensitive.
- Characters are compared as UTF-16 code units, not Unicode scalar values or grapheme clusters.
- Whitespace and punctuation participate like any other character.
- Runtime is `O(m × n)` for input lengths `m` and `n`.
- Working memory is `O(min(m, n))`.

Call the static methods directly; no dependency-injection registration is required. Both inputs must be non-null. Normalize casing or Unicode representation before calling if your application requires those equivalences.

Use the raw distance for edit-count thresholds and ranking. Use the normalized score or percentage when comparing pairs with different lengths on a common scale.
