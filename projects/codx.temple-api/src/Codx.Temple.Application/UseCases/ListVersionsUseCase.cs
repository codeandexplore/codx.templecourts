using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListVersionsUseCase
{
    private readonly IAppDbContext _db;

    public ListVersionsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<LessonVersionDto>> ExecuteAsync(Guid lessonKey, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Key == lessonKey, cancellationToken);

        if (lesson is null)
            throw new NotFoundException(nameof(Domain.Entities.Lesson), lessonKey);

        var versions = await _db.LessonVersions
            .Where(lv => lv.LessonId == lesson.Id)
            .OrderByDescending(lv => lv.CreatedAt)
            .Select(lv => new LessonVersionDto(
                lv.Id,
                lv.LessonId,
                lv.VersionNumber,
                lv.Status.ToString(),
                lv.ChangeNotes,
                lv.PublishedAt,
                lv.CreatedAt,
                new List<LessonNodeDto>()))
            .ToListAsync(cancellationToken);

        return versions;
    }
}
