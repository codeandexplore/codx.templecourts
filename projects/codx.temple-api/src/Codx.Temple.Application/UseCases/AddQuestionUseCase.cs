using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class AddQuestionUseCase
{
    private readonly IAppDbContext _db;

    public AddQuestionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<QuestionDto> ExecuteAsync(Guid nodeKey, CreateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var node = await _db.LessonNodes
            .Include(n => n.ChildNodes)
            .FirstOrDefaultAsync(n => n.Key == nodeKey, cancellationToken);

        if (node is null)
            throw new NotFoundException(nameof(LessonNode), nodeKey);

        if (node.ChildNodes.Count > 0)
            throw new ConflictException("Cannot add a question to a node that has child nodes. Only leaf nodes can hold questions.");

        if (!Enum.TryParse<QuestionType>(request.QuestionType, true, out var questionType))
            throw new ConflictException($"Invalid question type: {request.QuestionType}.");

        var order = request.Order ?? await _db.Questions
            .CountAsync(q => q.LessonNodeId == node.Id, cancellationToken);

        var question = Question.Create(node.Id, order, questionType, request.PromptText, request.Metadata, request.ReferenceContext);
        _db.Questions.Add(question);
        await _db.SaveChangesAsync(cancellationToken);

        return new QuestionDto(
            question.Id, question.Key, question.LessonNodeId, question.Order,
            question.QuestionType.ToString(), question.PromptText,
            question.Metadata, question.ReferenceContext);
    }
}
