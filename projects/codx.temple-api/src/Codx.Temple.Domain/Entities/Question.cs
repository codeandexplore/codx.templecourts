using System.Text.Json;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class Question
{
    public Guid Id { get; private set; }
    public Guid Key { get; private set; }
    public Guid LessonNodeId { get; private set; }
    public int Order { get; private set; }
    public QuestionType QuestionType { get; private set; }
    public string PromptText { get; private set; } = null!;
    public JsonDocument? Metadata { get; private set; }
    public JsonDocument? ReferenceContext { get; private set; }

    public LessonNode LessonNode { get; private set; } = null!;

    private Question() { }

    public static Question Create(
        Guid lessonNodeId,
        int order,
        QuestionType questionType,
        string promptText,
        JsonDocument? metadata = null,
        JsonDocument? referenceContext = null)
    {
        return new Question
        {
            Id = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            LessonNodeId = lessonNodeId,
            Order = order,
            QuestionType = questionType,
            PromptText = promptText,
            Metadata = metadata,
            ReferenceContext = referenceContext
        };
    }

    public void Update(string promptText, JsonDocument? metadata, JsonDocument? referenceContext)
    {
        PromptText = promptText;
        Metadata = metadata;
        ReferenceContext = referenceContext;
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
