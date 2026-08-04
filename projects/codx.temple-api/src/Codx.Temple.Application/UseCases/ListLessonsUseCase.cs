using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListLessonsUseCase
{
    private readonly IAppDbContext _db;

    public ListLessonsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<LessonDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Lessons
            .OrderBy(l => l.Number)
            .Select(l => new LessonDto(l.Id, l.Key, l.Number, l.Title, l.Status.ToString(), l.CurrentPublishedVersionId))
            .ToListAsync(cancellationToken);
    }
}
