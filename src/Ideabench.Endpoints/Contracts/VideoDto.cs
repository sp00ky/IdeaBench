using System;
using System.Collections.Generic;

namespace Ideabench.Endpoints.Contracts;

public sealed class VideoDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }
}
