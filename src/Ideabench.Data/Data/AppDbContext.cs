using System;
using Ideabench.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ideabench.Data.Data;

public sealed class AppDbContext : DbContext
{
    public const int MaxTitleLength = 200;
    public const int MaxVideoUrlLength = 800;
    public const int MaxThumbnailUrlLength = 500;
    public const int MaxSummaryLength = 8000;
    public const int MaxTagLength = 60;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<VideoEntry> VideoEntries => Set<VideoEntry>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<VideoTag> VideoTags => Set<VideoTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VideoEntry>(entity =>
        {
            entity.HasKey(video => video.Id);
            entity.Property(video => video.Title)
                .HasMaxLength(MaxTitleLength)
                .IsRequired();
            entity.Property(video => video.VideoUrl)
                .HasMaxLength(MaxVideoUrlLength)
                .IsRequired();
            entity.Property(video => video.ThumbnailUrl)
                .HasMaxLength(MaxThumbnailUrlLength)
                .IsRequired();
            entity.Property(video => video.Summary)
                .HasMaxLength(MaxSummaryLength)
                .IsRequired();
            entity.HasIndex(video => video.Title);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(tag => tag.Id);
            entity.Property(tag => tag.Name)
                .HasMaxLength(MaxTagLength)
                .IsRequired();
            entity.HasIndex(tag => tag.Name)
                .IsUnique();
        });

        modelBuilder.Entity<VideoTag>(entity =>
        {
            entity.HasKey(videoTag => new { videoTag.VideoEntryId, videoTag.TagId });
            entity.HasOne(videoTag => videoTag.VideoEntry)
                .WithMany(video => video.VideoTags)
                .HasForeignKey(videoTag => videoTag.VideoEntryId);
            entity.HasOne(videoTag => videoTag.Tag)
                .WithMany(tag => tag.VideoTags)
                .HasForeignKey(videoTag => videoTag.TagId);
        });
    }
}
