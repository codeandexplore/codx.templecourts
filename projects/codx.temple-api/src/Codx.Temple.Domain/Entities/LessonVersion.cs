using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class LessonVersion
{
    public Guid Id { get; private set; }
    public Guid LessonId { get; private set; }
    public int VersionNumber { get; private set; }
    public LessonVersionStatus Status { get; private set; }
    public Guid? ClonedFromVersionId { get; private set; }
    public string? ChangeNotes { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Lesson Lesson { get; private set; } = null!;
    public LessonVersion? ClonedFromVersion { get; private set; }
    public ICollection<LessonNode> Nodes { get; private set; } = new List<LessonNode>();

    private LessonVersion() { }

    public static LessonVersion Create(Guid lessonId, int versionNumber, string? changeNotes = null, Guid? clonedFromVersionId = null)
    {
        return new LessonVersion
        {
            Id = Guid.NewGuid(),
            LessonId = lessonId,
            VersionNumber = versionNumber,
            Status = LessonVersionStatus.Draft,
            ClonedFromVersionId = clonedFromVersionId,
            ChangeNotes = changeNotes,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Publish()
    {
        Status = LessonVersionStatus.Published;
        PublishedAt = DateTimeOffset.UtcNow;
    }

    public void Retire()
    {
        Status = LessonVersionStatus.Retired;
    }
}
