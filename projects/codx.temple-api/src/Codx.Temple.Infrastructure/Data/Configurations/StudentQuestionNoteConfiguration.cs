using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class StudentQuestionNoteConfiguration : IEntityTypeConfiguration<StudentQuestionNote>
{
    public void Configure(EntityTypeBuilder<StudentQuestionNote> builder)
    {
        builder.HasKey(n => n.Id);

        builder.HasIndex(n => new { n.StudentId, n.QuestionKey }).IsUnique();

        builder.Property(n => n.NoteText).IsRequired();

        builder.HasOne(n => n.Student)
            .WithMany()
            .HasForeignKey(n => n.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
