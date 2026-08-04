using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class ThreadMessageConfiguration : IEntityTypeConfiguration<ThreadMessage>
{
    public void Configure(EntityTypeBuilder<ThreadMessage> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.BodyText).IsRequired();
        builder.HasOne(m => m.AnswerThread).WithMany(t => t.Messages).HasForeignKey(m => m.AnswerThreadId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.Author).WithMany().HasForeignKey(m => m.AuthorId).OnDelete(DeleteBehavior.Cascade);
    }
}
