using FastEndpoints;
using Ideabench.Endpoints.Contracts;
using Ideabench.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Ideabench.Endpoints.Tags;

public sealed class ListTagsEndpoint : EndpointWithoutRequest<List<TagDto>>
{
    private readonly AppDbContext _dbContext;

    public ListTagsEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/tags");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var tags = await _dbContext.Tags
            .AsNoTracking()
            .OrderBy(tag => tag.Name)
            .Select(tag => new TagDto { Name = tag.Name })
            .ToListAsync(cancellationToken);

        await Send.OkAsync(tags, cancellationToken);
    }
}
