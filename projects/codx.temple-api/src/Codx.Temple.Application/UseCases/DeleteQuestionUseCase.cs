using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class DeleteQuestionUseCase
{
    private readonly IAppDbContext _db;

    public DeleteQuestionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task ExecuteAsync(Guid nodeKey, Guid questionKey, CancellationToken cancellationToken = default)
    {
        var question = await _db.Questions
            .FirstOrDefaultAsync(q => q.Key == questionKey && q.LessonNode.Key == nodeKey, cancellationToken);

        if (question is null)
            throw new NotFoundException(nameof(Domain.Entities.Question), questionKey);

        _db.Questions.Remove(question);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
