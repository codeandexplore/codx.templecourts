using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class LessonVersionConfiguration : IEntityTypeConfiguration<LessonVersion>
{
    public void Configure(EntityTypeBuilder<LessonVersion> builder)
    {
        builder.HasKey(lv => lv.Id);

        builder.HasOne(lv => lv.Lesson)
            .WithMany(l => l.Versions)
            .HasForeignKey(lv => lv.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lv => lv.ClonedFromVersion)
            .WithMany()
            .HasForeignKey(lv => lv.ClonedFromVersionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(lv => lv.ChangeNotes).HasMaxLength(500);
        builder.Property(lv => lv.Status).HasConversion<string>();
    }
}
