using FastEndpoints;
using Ideabench.Endpoints.Contracts;
using Ideabench.Endpoints.Mapping;
using Ideabench.Endpoints.Support;
using Ideabench.Data.Data;
using Ideabench.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ideabench.Endpoints.Videos;

public sealed class UpdateVideoEndpoint : Endpoint<UpdateVideoRequest, VideoDto>
{
    private readonly AppDbContext _dbContext;

    public UpdateVideoEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Put("/api/videos/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateVideoRequest request, CancellationToken cancellationToken)
    {
        var video = await _dbContext.VideoEntries
            .Include(entry => entry.VideoTags)
            .ThenInclude(videoTag => videoTag.Tag)
            .FirstOrDefaultAsync(entry => entry.Id == request.Id, cancellationToken);

        if (video is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        video.Title = request.Title.Trim();
        video.VideoUrl = request.VideoUrl.Trim();
        video.ThumbnailUrl = request.ThumbnailUrl.Trim();
        video.Summary = request.Summary.Trim();
        video.UpdatedUtc = DateTime.UtcNow;

        var desiredTags = TagNormalizer.NormalizeTags(request.Tags);
        var existingTags = await _dbContext.Tags
            .Where(tag => desiredTags.Contains(tag.Name))
            .ToListAsync(cancellationToken);

        var existingTagNames = existingTags
            .Select(tag => tag.Name)
            .ToHashSet();

        var newTags = desiredTags
            .Where(name => !existingTagNames.Contains(name))
            .Select(name => new Tag { Id = Guid.NewGuid(), Name = name })
            .ToList();

        if (newTags.Count > 0)
        {
            _dbContext.Tags.AddRange(newTags);
        }

        var desiredTagSet = desiredTags.ToHashSet();
        var tagsToRemove = video.VideoTags
            .Where(videoTag => videoTag.Tag is not null && !desiredTagSet.Contains(videoTag.Tag.Name))
            .ToList();

        foreach (var videoTag in tagsToRemove)
        {
            video.VideoTags.Remove(videoTag);
        }

        var currentTagSet = video.VideoTags
            .Where(videoTag => videoTag.Tag is not null)
            .Select(videoTag => videoTag.Tag!.Name)
            .ToHashSet();

        foreach (var tag in existingTags.Concat(newTags))
        {
            if (!currentTagSet.Contains(tag.Name))
            {
                video.VideoTags.Add(new VideoTag
                {
                    VideoEntryId = video.Id,
                    TagId = tag.Id,
                    Tag = tag
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(video)
            .Collection(entry => entry.VideoTags)
            .Query()
            .Include(videoTag => videoTag.Tag)
            .LoadAsync(cancellationToken);

        await Send.OkAsync(video.ToDto(), cancellationToken);
    }
}
