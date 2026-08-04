using Microsoft.EntityFrameworkCore;
using Codx.Temple.Domain.Entities;

namespace Codx.Temple.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<RoleAssignment> RoleAssignments { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<LessonVersion> LessonVersions { get; }
    DbSet<LessonNode> LessonNodes { get; }
    DbSet<Question> Questions { get; }
    DbSet<LessonAttempt> LessonAttempts { get; }
    DbSet<StudentAnswer> StudentAnswers { get; }
    DbSet<StudentQuestionNote> StudentQuestionNotes { get; }
    DbSet<TeacherAssignment> TeacherAssignments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
