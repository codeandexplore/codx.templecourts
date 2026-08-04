namespace Codx.Temple.Domain.Entities;

public class LessonNode
{
    public Guid Id { get; private set; }
    public Guid Key { get; private set; }
    public Guid LessonVersionId { get; private set; }
    public Guid? ParentNodeId { get; private set; }
    public int Depth { get; private set; }
    public int Order { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool RequiresPriorSiblingAnswered { get; private set; }

    public LessonVersion LessonVersion { get; private set; } = null!;
    public LessonNode? ParentNode { get; private set; }
    public ICollection<LessonNode> ChildNodes { get; private set; } = new List<LessonNode>();
    public ICollection<Question> Questions { get; private set; } = new List<Question>();

    private LessonNode() { }

    public static LessonNode Create(
        Guid lessonVersionId,
        Guid? parentNodeId,
        int depth,
        int order,
        string title,
        string description,
        bool requiresPriorSiblingAnswered = false)
    {
        return new LessonNode
        {
            Id = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            LessonVersionId = lessonVersionId,
            ParentNodeId = parentNodeId,
            Depth = depth,
            Order = order,
            Title = title,
            Description = description,
            RequiresPriorSiblingAnswered = requiresPriorSiblingAnswered
        };
    }

    public void Update(string title, string description, bool requiresPriorSiblingAnswered)
    {
        Title = title;
        Description = description;
        RequiresPriorSiblingAnswered = requiresPriorSiblingAnswered;
    }

    public void SetOrder(int order)
    {
        Order = order;
    }

    public void SetKey(Guid key)
    {
        Key = key;
    }
}
