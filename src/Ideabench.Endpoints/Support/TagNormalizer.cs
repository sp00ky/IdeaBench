using System.Collections.Generic;
using System.Linq;

namespace Ideabench.Endpoints.Support;

public static class TagNormalizer
{
    public static List<string> NormalizeTags(IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return new List<string>();
        }

        return tags
            .Select(tag => tag?.Trim())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag!.ToLowerInvariant())
            .Distinct()
            .ToList();
    }
}
