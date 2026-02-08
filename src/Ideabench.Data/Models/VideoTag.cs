using System;

namespace Ideabench.Data.Models;

public sealed class VideoTag
{
    public Guid VideoEntryId { get; set; }

    public VideoEntry? VideoEntry { get; set; }

    public Guid TagId { get; set; }

    public Tag? Tag { get; set; }
}
