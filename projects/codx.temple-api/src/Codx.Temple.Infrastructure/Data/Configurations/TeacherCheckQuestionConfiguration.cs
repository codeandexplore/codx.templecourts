using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class TeacherCheckQuestionConfiguration : IEntityTypeConfiguration<TeacherCheckQuestion>
{
    public void Configure(EntityTypeBuilder<TeacherCheckQuestion> builder)
    {
        builder.HasKey(q => q.Id);
        builder.HasIndex(q => new { q.TeacherId, q.QuestionKey });
        builder.Property(q => q.NoteText).IsRequired();
        builder.HasOne(q => q.Teacher).WithMany().HasForeignKey(q => q.TeacherId).OnDelete(DeleteBehavior.Cascade);
    }
}
