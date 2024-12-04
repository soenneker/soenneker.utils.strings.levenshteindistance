[![](https://img.shields.io/nuget/v/soenneker.utils.strings.levenshteindistance.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.strings.levenshteindistance/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.strings.levenshteindistance/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.strings.levenshteindistance/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.strings.levenshteindistance.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.strings.levenshteindistance/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Strings.LevenshteinDistance
### A utility library for comparing strings via the Levenshtein Distance algorithm

## Installation

```
dotnet add package Soenneker.Utils.Strings.LevenshteinDistance
```

## Why Levenshtein Distance?

Levenshtein Distance, also known as **Edit Distance**, is a widely used metric for measuring the similarity between two strings. It calculates the minimum number of operations required to transform one string into the other, where operations include:

- **Insertion** of a character.
- **Deletion** of a character.
- **Substitution** of a character.

Levenshtein Distance is particularly useful in applications like:

### Flexible String Comparison:
It handles strings of **unequal length** and allows for more flexible comparisons.

### Real-World Scenarios:
It's well-suited for tasks like:
- Fuzzy string matching.
- Spelling correction.
- DNA sequence analysis.
- Natural language processing.

### Position-Aware:
It captures positional changes and structural differences more effectively than set-based metrics like Jaccard or Hamming Distance.

### Comprehensive Error Handling:
Unlike simpler metrics, it accounts for insertions and deletions, making it robust for strings with typos or omissions.

---

## Usage

```csharp
var text1 = "kitten";
var text2 = "sitting";

int distance = LevenshteinDistanceUtil.ComputeDistance(text1, text2); // 3
double similarityPercentage = LevenshteinDistanceUtil.CalculatePercentage(text1, text2); // ~57.14
```

This library is efficient, straightforward, and ideal for handling real-world string similarity comparisons where flexibility and accuracy are key.