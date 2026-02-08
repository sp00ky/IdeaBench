using FastEndpoints;
using Ideabench.Endpoints.Contracts;
using Ideabench.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Ideabench.Endpoints.Videos;

public sealed class DeleteVideoEndpoint : Endpoint<DeleteVideoRequest, EmptyResponse>
{
    private readonly AppDbContext _dbContext;

    public DeleteVideoEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Delete("/api/videos/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteVideoRequest request, CancellationToken cancellationToken)
    {
        var video = await _dbContext.VideoEntries
            .Include(entry => entry.VideoTags)
            .FirstOrDefaultAsync(entry => entry.Id == request.Id, cancellationToken);

        if (video is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        _dbContext.VideoTags.RemoveRange(video.VideoTags);
        _dbContext.VideoEntries.Remove(video);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await Send.NoContentAsync(cancellationToken);
    }
}
