using Codx.Temple.Application.Abstractions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonVersion> LessonVersions => Set<LessonVersion>();
    public DbSet<LessonNode> LessonNodes => Set<LessonNode>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<LessonAttempt> LessonAttempts => Set<LessonAttempt>();
    public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();
    public DbSet<StudentQuestionNote> StudentQuestionNotes => Set<StudentQuestionNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
