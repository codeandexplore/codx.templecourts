using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class CreateLessonUseCase
{
    private readonly IAppDbContext _db;

    public CreateLessonUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<LessonDto> ExecuteAsync(CreateLessonRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Lessons.AnyAsync(l => l.Number == request.Number, cancellationToken);
        if (exists)
            throw new ConflictException($"A lesson with number {request.Number} already exists.");

        var lesson = Lesson.Create(request.Number, request.Title);
        _db.Lessons.Add(lesson);
        await _db.SaveChangesAsync(cancellationToken);

        return new LessonDto(lesson.Id, lesson.Key, lesson.Number, lesson.Title, lesson.Status.ToString(), lesson.CurrentPublishedVersionId);
    }
}
