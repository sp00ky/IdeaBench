using System.Linq;
using Ideabench.Endpoints.Contracts;
using Ideabench.Data.Models;

namespace Ideabench.Endpoints.Mapping;

public static class VideoMapping
{
    public static VideoDto ToDto(this VideoEntry entry)
    {
        return new VideoDto
        {
            Id = entry.Id,
            Title = entry.Title,
            VideoUrl = entry.VideoUrl,
            ThumbnailUrl = entry.ThumbnailUrl,
            Summary = entry.Summary,
            CreatedUtc = entry.CreatedUtc,
            UpdatedUtc = entry.UpdatedUtc,
            Tags = entry.VideoTags
                .Select(videoTag => videoTag.Tag?.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!)
                .OrderBy(name => name)
                .ToList()
        };
    }
}
