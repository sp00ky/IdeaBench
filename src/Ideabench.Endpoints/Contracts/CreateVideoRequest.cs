using System.Collections.Generic;

namespace Ideabench.Endpoints.Contracts;

public sealed class CreateVideoRequest
{
    public string Title { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();
}
