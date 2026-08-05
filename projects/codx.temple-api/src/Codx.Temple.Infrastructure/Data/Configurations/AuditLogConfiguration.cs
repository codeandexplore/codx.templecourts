using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Action).IsRequired().HasMaxLength(100);
        builder.HasOne(l => l.PerformedBy).WithMany().HasForeignKey(l => l.PerformedById).OnDelete(DeleteBehavior.Cascade);
    }
}
