using FastEndpoints;
using Ideabench.Endpoints.Contracts;
using Ideabench.Endpoints.Mapping;
using Ideabench.Endpoints.Support;
using Ideabench.Data.Data;
using Ideabench.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ideabench.Endpoints.Videos;

public sealed class CreateVideoEndpoint : Endpoint<CreateVideoRequest, VideoDto>
{
    private readonly AppDbContext _dbContext;

    public CreateVideoEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/videos");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateVideoRequest request, CancellationToken cancellationToken)
    {
        var tagNames = TagNormalizer.NormalizeTags(request.Tags);
        var existingTags = await _dbContext.Tags
            .Where(tag => tagNames.Contains(tag.Name))
            .ToListAsync(cancellationToken);

        var existingTagNames = existingTags
            .Select(tag => tag.Name)
            .ToHashSet();

        var newTags = tagNames
            .Where(name => !existingTagNames.Contains(name))
            .Select(name => new Tag { Id = Guid.NewGuid(), Name = name })
            .ToList();

        if (newTags.Count > 0)
        {
            _dbContext.Tags.AddRange(newTags);
        }

        var now = DateTime.UtcNow;

        var video = new VideoEntry
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            VideoUrl = request.VideoUrl.Trim(),
            ThumbnailUrl = request.ThumbnailUrl.Trim(),
            Summary = request.Summary.Trim(),
            CreatedUtc = now,
            UpdatedUtc = now
        };

        foreach (var tag in existingTags.Concat(newTags))
        {
            video.VideoTags.Add(new VideoTag
            {
                VideoEntryId = video.Id,
                TagId = tag.Id,
                Tag = tag
            });
        }

        _dbContext.VideoEntries.Add(video);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(video)
            .Collection(entry => entry.VideoTags)
            .Query()
            .Include(videoTag => videoTag.Tag)
            .LoadAsync(cancellationToken);

        await Send.OkAsync(video.ToDto(), cancellationToken);
    }
}
