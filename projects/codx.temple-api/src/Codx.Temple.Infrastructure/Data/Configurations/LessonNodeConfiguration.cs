using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class LessonNodeConfiguration : IEntityTypeConfiguration<LessonNode>
{
    public void Configure(EntityTypeBuilder<LessonNode> builder)
    {
        builder.HasKey(ln => ln.Id);

        builder.HasIndex(ln => ln.Key);

        builder.Property(ln => ln.Title).IsRequired().HasMaxLength(200);
        builder.Property(ln => ln.Description).IsRequired().HasMaxLength(2000);

        builder.HasOne(ln => ln.LessonVersion)
            .WithMany(lv => lv.Nodes)
            .HasForeignKey(ln => ln.LessonVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ln => ln.ParentNode)
            .WithMany(ln => ln.ChildNodes)
            .HasForeignKey(ln => ln.ParentNodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
