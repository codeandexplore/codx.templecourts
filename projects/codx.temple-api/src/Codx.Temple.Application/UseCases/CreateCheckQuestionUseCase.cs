using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class CreateCheckQuestionUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public CreateCheckQuestionUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<TeacherCheckQuestionDto> ExecuteAsync(CreateCheckQuestionRequest request, CancellationToken ct = default)
    {
        var q = TeacherCheckQuestion.Create(_currentUser.UserId, request.QuestionKey, request.NoteText);
        _db.TeacherCheckQuestions.Add(q);
        await _db.SaveChangesAsync(ct);
        return new TeacherCheckQuestionDto(q.Id, q.QuestionKey, q.NoteText, false, q.CreatedAt);
    }
}
