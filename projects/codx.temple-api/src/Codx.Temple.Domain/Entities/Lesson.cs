using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class Lesson
{
    public Guid Id { get; private set; }
    public Guid Key { get; private set; }
    public int Number { get; private set; }
    public string Title { get; private set; } = null!;
    public Guid? CurrentPublishedVersionId { get; private set; }
    public LessonStatus Status { get; private set; }

    public ICollection<LessonVersion> Versions { get; private set; } = new List<LessonVersion>();
    public LessonVersion? CurrentPublishedVersion { get; private set; }

    private Lesson() { }

    public static Lesson Create(int number, string title)
    {
        return new Lesson
        {
            Id = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Number = number,
            Title = title,
            Status = LessonStatus.Active
        };
    }

    public void SetCurrentPublishedVersion(Guid versionId)
    {
        CurrentPublishedVersionId = versionId;
    }

    public void Update(int number, string title)
    {
        Number = number;
        Title = title;
    }

    public void Archive()
    {
        Status = LessonStatus.Archived;
    }
}
