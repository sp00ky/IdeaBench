using FastEndpoints;
using FluentValidation;
using Ideabench.Endpoints.Contracts;
using Ideabench.Data.Data;

namespace Ideabench.Endpoints.Videos;

public sealed class CreateVideoValidator : Validator<CreateVideoRequest>
{
    public CreateVideoValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(AppDbContext.MaxTitleLength);

        RuleFor(request => request.VideoUrl)
            .NotEmpty()
            .MaximumLength(AppDbContext.MaxVideoUrlLength);

        RuleFor(request => request.ThumbnailUrl)
            .NotEmpty()
            .MaximumLength(AppDbContext.MaxThumbnailUrlLength);

        RuleFor(request => request.Summary)
            .NotEmpty()
            .MaximumLength(AppDbContext.MaxSummaryLength);

        RuleForEach(request => request.Tags)
            .NotEmpty()
            .MaximumLength(AppDbContext.MaxTagLength);

        RuleFor(request => request.Tags)
            .Must(tags => tags is null || tags.Count <= 20)
            .WithMessage("No more than 20 tags are allowed.");
    }
}
