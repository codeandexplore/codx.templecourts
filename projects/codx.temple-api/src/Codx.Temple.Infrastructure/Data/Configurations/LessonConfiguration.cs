using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);

        builder.HasIndex(l => l.Key).IsUnique();

        builder.Property(l => l.Title).IsRequired().HasMaxLength(200);

        builder.HasOne(l => l.CurrentPublishedVersion)
            .WithMany()
            .HasForeignKey(l => l.CurrentPublishedVersionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(l => l.Status).HasConversion<string>();
    }
}
