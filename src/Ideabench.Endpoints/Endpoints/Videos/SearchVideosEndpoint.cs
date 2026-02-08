using FastEndpoints;
using Ideabench.Endpoints.Contracts;
using Ideabench.Endpoints.Mapping;
using Ideabench.Endpoints.Support;
using Ideabench.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Ideabench.Endpoints.Videos;

public sealed class SearchVideosEndpoint : Endpoint<SearchVideosRequest, List<VideoDto>>
{
    private readonly AppDbContext _dbContext;

    public SearchVideosEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/videos");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SearchVideosRequest request, CancellationToken cancellationToken)
    {
        var queryable = _dbContext.VideoEntries
            .AsNoTracking()
            .Include(entry => entry.VideoTags)
            .ThenInclude(videoTag => videoTag.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.Trim();
            queryable = queryable.Where(entry =>
                EF.Functions.Like(entry.Title, $"%{term}%") ||
                EF.Functions.Like(entry.Summary, $"%{term}%"));
        }

        var tags = TagNormalizer.NormalizeTags(request.Tags);
        if (tags.Count > 0)
        {
            queryable = queryable.Where(entry =>
                entry.VideoTags.Any(videoTag => videoTag.Tag != null && tags.Contains(videoTag.Tag.Name)));
        }

        var skip = Math.Max(request.Skip ?? 0, 0);
        var take = Math.Clamp(request.Take ?? 50, 1, 200);

        var results = await queryable
            .OrderByDescending(entry => entry.UpdatedUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        var response = results
            .Select(entry => entry.ToDto())
            .ToList();

        await Send.OkAsync(response, cancellationToken);
    }
}
