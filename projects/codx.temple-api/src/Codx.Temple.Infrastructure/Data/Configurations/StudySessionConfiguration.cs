using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class StudySessionConfiguration : IEntityTypeConfiguration<StudySession>
{
    public void Configure(EntityTypeBuilder<StudySession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Status).HasConversion<string>();

        builder.HasOne(s => s.LessonAttempt)
            .WithMany()
            .HasForeignKey(s => s.LessonAttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
