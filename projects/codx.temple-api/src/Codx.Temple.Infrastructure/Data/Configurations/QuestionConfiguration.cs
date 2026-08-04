using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);

        builder.HasIndex(q => q.Key);

        builder.Property(q => q.PromptText).IsRequired().HasMaxLength(1000);
        builder.Property(q => q.QuestionType).HasConversion<string>();

        builder.Property(q => q.Metadata).HasColumnType("jsonb");
        builder.Property(q => q.ReferenceContext).HasColumnType("jsonb");

        builder.HasOne(q => q.LessonNode)
            .WithMany(ln => ln.Questions)
            .HasForeignKey(q => q.LessonNodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
