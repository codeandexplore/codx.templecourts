using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class UpdateLessonUseCase
{
    private readonly IAppDbContext _db;

    public UpdateLessonUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<LessonDto> ExecuteAsync(Guid key, UpdateLessonRequest request, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Key == key, cancellationToken);

        if (lesson is null)
            throw new NotFoundException(nameof(Domain.Entities.Lesson), key);

        var exists = await _db.Lessons.AnyAsync(l => l.Number == request.Number && l.Key != key, cancellationToken);
        if (exists)
            throw new ConflictException($"A lesson with number {request.Number} already exists.");

        lesson.Update(request.Number, request.Title);

        await _db.SaveChangesAsync(cancellationToken);

        return new LessonDto(lesson.Id, lesson.Key, lesson.Number, lesson.Title, lesson.Status.ToString(), lesson.CurrentPublishedVersionId);
    }
}
