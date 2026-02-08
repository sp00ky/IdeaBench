using System.Collections.Generic;

namespace Ideabench.Endpoints.Contracts;

public sealed class SearchVideosRequest
{
    public string? Query { get; set; }

    public List<string>? Tags { get; set; }

    public int? Skip { get; set; }

    public int? Take { get; set; }
}
