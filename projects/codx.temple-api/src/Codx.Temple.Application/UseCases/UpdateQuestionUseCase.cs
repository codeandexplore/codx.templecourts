using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class UpdateQuestionUseCase
{
    private readonly IAppDbContext _db;

    public UpdateQuestionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<QuestionDto> ExecuteAsync(Guid nodeKey, Guid questionKey, UpdateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var question = await _db.Questions
            .FirstOrDefaultAsync(q => q.Key == questionKey && q.LessonNode.Key == nodeKey, cancellationToken);

        if (question is null)
            throw new NotFoundException(nameof(Domain.Entities.Question), questionKey);

        question.Update(request.PromptText, request.Metadata, request.ReferenceContext);
        await _db.SaveChangesAsync(cancellationToken);

        return new QuestionDto(
            question.Id, question.Key, question.LessonNodeId, question.Order,
            question.QuestionType.ToString(), question.PromptText,
            question.Metadata, question.ReferenceContext);
    }
}
