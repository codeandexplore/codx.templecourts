using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ReorderQuestionsUseCase
{
    private readonly IAppDbContext _db;

    public ReorderQuestionsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task ExecuteAsync(Guid nodeKey, ReorderQuestionsRequest request, CancellationToken cancellationToken = default)
    {
        var questions = await _db.Questions
            .Where(q => q.LessonNode.Key == nodeKey)
            .ToListAsync(cancellationToken);

        for (int i = 0; i < request.OrderedKeys.Count; i++)
        {
            var question = questions.FirstOrDefault(q => q.Key == request.OrderedKeys[i]);
            if (question is not null)
                question.SetOrder(i);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
