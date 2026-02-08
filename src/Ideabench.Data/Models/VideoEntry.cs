using System;
using System.Collections.Generic;

namespace Ideabench.Data.Models;

public sealed class VideoEntry
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public List<VideoTag> VideoTags { get; set; } = new();
}
