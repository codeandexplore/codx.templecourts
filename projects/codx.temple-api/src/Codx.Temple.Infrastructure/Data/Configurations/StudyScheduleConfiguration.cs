using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codx.Temple.Infrastructure.Data.Configurations;

public class StudyScheduleConfiguration : IEntityTypeConfiguration<StudySchedule>
{
    public void Configure(EntityTypeBuilder<StudySchedule> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Status).HasConversion<string>();
        builder.Property(s => s.MeetingLink).HasMaxLength(500);
        builder.HasOne(s => s.Student).WithMany().HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.Teacher).WithMany().HasForeignKey(s => s.TeacherId).OnDelete(DeleteBehavior.Cascade);
    }
}
