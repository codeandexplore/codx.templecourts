using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ArchiveLessonUseCase
{
    private readonly IAppDbContext _db;

    public ArchiveLessonUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<LessonDto> ExecuteAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Key == key, cancellationToken);

        if (lesson is null)
            throw new NotFoundException(nameof(Domain.Entities.Lesson), key);

        lesson.Archive();
        await _db.SaveChangesAsync(cancellationToken);

        return new LessonDto(lesson.Id, lesson.Key, lesson.Number, lesson.Title, lesson.Status.ToString(), lesson.CurrentPublishedVersionId);
    }
}
