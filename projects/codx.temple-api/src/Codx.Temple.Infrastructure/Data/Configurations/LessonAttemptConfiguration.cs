using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class LessonAttemptConfiguration : IEntityTypeConfiguration<LessonAttempt>
{
    public void Configure(EntityTypeBuilder<LessonAttempt> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.StudentId, a.LessonKey });

        builder.Property(a => a.Status).HasConversion<string>();

        builder.HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.LessonVersion)
            .WithMany()
            .HasForeignKey(a => a.LessonVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
