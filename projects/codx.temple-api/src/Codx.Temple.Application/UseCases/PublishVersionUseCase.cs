using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class PublishVersionUseCase
{
    private readonly IAppDbContext _db;

    public PublishVersionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task ExecuteAsync(Guid lessonKey, Guid versionId, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Key == lessonKey, cancellationToken);

        if (lesson is null)
            throw new NotFoundException(nameof(Domain.Entities.Lesson), lessonKey);

        var version = await _db.LessonVersions
            .Include(lv => lv.Nodes)
            .FirstOrDefaultAsync(lv => lv.Id == versionId && lv.LessonId == lesson.Id, cancellationToken);

        if (version is null)
            throw new NotFoundException(nameof(Domain.Entities.LessonVersion), versionId);

        if (version.Status != LessonVersionStatus.Draft)
            throw new ConflictException("Only draft versions can be published.");

        if (!version.Nodes.Any(n => n.ParentNodeId == null))
            throw new ConflictException("A lesson must have at least one top-level node before publishing.");

        var previousPublished = await _db.LessonVersions
            .FirstOrDefaultAsync(lv => lv.LessonId == lesson.Id && lv.Status == LessonVersionStatus.Published, cancellationToken);

        if (previousPublished is not null)
            previousPublished.Retire();

        version.Publish();
        lesson.SetCurrentPublishedVersion(version.Id);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
