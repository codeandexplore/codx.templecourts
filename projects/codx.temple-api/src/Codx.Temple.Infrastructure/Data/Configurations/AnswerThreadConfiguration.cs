using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class AnswerThreadConfiguration : IEntityTypeConfiguration<AnswerThread>
{
    public void Configure(EntityTypeBuilder<AnswerThread> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Status).HasConversion<string>();
        builder.HasOne(t => t.StudentAnswer).WithOne().HasForeignKey<AnswerThread>(t => t.StudentAnswerId).OnDelete(DeleteBehavior.Cascade);
    }
}
