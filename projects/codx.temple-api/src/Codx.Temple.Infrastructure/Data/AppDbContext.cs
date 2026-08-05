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
    public DbSet<TeacherAssignment> TeacherAssignments => Set<TeacherAssignment>();
    public DbSet<StudySession> StudySessions => Set<StudySession>();
    public DbSet<AnswerFlag> AnswerFlags => Set<AnswerFlag>();
    public DbSet<TeacherCheckQuestion> TeacherCheckQuestions => Set<TeacherCheckQuestion>();
    public DbSet<AnswerThread> AnswerThreads => Set<AnswerThread>();
    public DbSet<ThreadMessage> ThreadMessages => Set<ThreadMessage>();
    public DbSet<StudySchedule> StudySchedules => Set<StudySchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
