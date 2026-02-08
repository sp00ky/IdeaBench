using FastEndpoints;
using Ideabench.Endpoints.Contracts;
using Ideabench.Endpoints.Mapping;
using Ideabench.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Ideabench.Endpoints.Videos;

public sealed class GetVideoEndpoint : Endpoint<GetVideoRequest, VideoDto>
{
    private readonly AppDbContext _dbContext;

    public GetVideoEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/videos/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetVideoRequest request, CancellationToken cancellationToken)
    {
        var video = await _dbContext.VideoEntries
            .AsNoTracking()
            .Include(entry => entry.VideoTags)
            .ThenInclude(videoTag => videoTag.Tag)
            .FirstOrDefaultAsync(entry => entry.Id == request.Id, cancellationToken);

        if (video is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(video.ToDto(), cancellationToken);
    }
}
