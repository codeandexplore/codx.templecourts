using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
{
    public void Configure(EntityTypeBuilder<StudentAnswer> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.LessonAttemptId, a.QuestionKey }).IsUnique();

        builder.Property(a => a.AnswerValue)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(a => a.PromptSnapshot).IsRequired();
        builder.Property(a => a.QuestionTypeSnapshot).IsRequired();

        builder.HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.LessonAttempt)
            .WithMany(la => la.Answers)
            .HasForeignKey(a => a.LessonAttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
