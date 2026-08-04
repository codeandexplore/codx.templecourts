using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class AnswerFlagConfiguration : IEntityTypeConfiguration<AnswerFlag>
{
    public void Configure(EntityTypeBuilder<AnswerFlag> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FlagType).HasConversion<string>();

        builder.HasIndex(f => new { f.StudentId, f.ResolvedAt });

        builder.HasOne(f => f.Student)
            .WithMany()
            .HasForeignKey(f => f.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.LessonAttempt)
            .WithMany()
            .HasForeignKey(f => f.LessonAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.RaisedInSession)
            .WithMany()
            .HasForeignKey(f => f.RaisedInSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
