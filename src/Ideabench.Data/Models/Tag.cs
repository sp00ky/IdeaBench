using System;
using System.Collections.Generic;

namespace Ideabench.Data.Models;

public sealed class Tag
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<VideoTag> VideoTags { get; set; } = new();
}
