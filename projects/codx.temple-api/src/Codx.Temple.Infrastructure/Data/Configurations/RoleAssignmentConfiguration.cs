using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class RoleAssignmentConfiguration : IEntityTypeConfiguration<RoleAssignment>
{
    public void Configure(EntityTypeBuilder<RoleAssignment> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.UserId, r.Role }).IsUnique();

        builder.HasOne(r => r.User)
            .WithMany(u => u.RoleAssignments)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Assigner)
            .WithMany()
            .HasForeignKey(r => r.AssignedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.Role).HasConversion<string>();
    }
}
