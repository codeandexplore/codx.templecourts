using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class TeacherAssignmentConfiguration : IEntityTypeConfiguration<TeacherAssignment>
{
    public void Configure(EntityTypeBuilder<TeacherAssignment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.StudentId);

        builder.Property(a => a.Status).HasConversion<string>();

        builder.HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.PrimaryTeacher)
            .WithMany()
            .HasForeignKey(a => a.PrimaryTeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.AssignedBy)
            .WithMany()
            .HasForeignKey(a => a.AssignedById)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
