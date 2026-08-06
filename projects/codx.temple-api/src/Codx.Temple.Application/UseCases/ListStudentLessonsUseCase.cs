using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListStudentLessonsUseCase
{
    private readonly IAppDbContext _db;

    public ListStudentLessonsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<LessonDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Lessons
            .Where(l => l.Status == LessonStatus.Active)
            .OrderBy(l => l.Number)
            .Select(l => new LessonDto(l.Id, l.Key, l.Number, l.Title, l.Status.ToString(), l.CurrentPublishedVersionId))
            .ToListAsync(cancellationToken);
    }
}
