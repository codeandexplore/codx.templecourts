using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentNotes;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Moq;

namespace Codx.Temple.Application.Tests;

public class CreateStudentQuestionNoteUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<ICurrentUserAccessor> _currentUserMock;
    private readonly CreateStudentQuestionNoteUseCase _useCase;

    public CreateStudentQuestionNoteUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _currentUserMock = new Mock<ICurrentUserAccessor>();
        _currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _useCase = new CreateStudentQuestionNoteUseCase(_dbMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateNote()
    {
        var questionKey = Guid.NewGuid();
        var request = new CreateNoteRequest("My note text");
        var notes = new List<StudentQuestionNote>();
        _dbMock.Setup(db => db.StudentQuestionNotes).Returns(DbSetMockHelper.CreateMockDbSet(notes).Object);

        var result = await _useCase.ExecuteAsync(questionKey, request);

        Assert.NotNull(result);
        Assert.Equal(questionKey, result.QuestionKey);
        Assert.Equal("My note text", result.NoteText);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowConflictWhenDuplicate()
    {
        var questionKey = Guid.NewGuid();
        var userId = _currentUserMock.Object.UserId;
        var existing = StudentQuestionNote.Create(userId, questionKey, "existing");
        var notes = new List<StudentQuestionNote> { existing };
        var request = new CreateNoteRequest("new");

        _dbMock.Setup(db => db.StudentQuestionNotes).Returns(DbSetMockHelper.CreateMockDbSet(notes).Object);

        await Assert.ThrowsAsync<ConflictException>(() => _useCase.ExecuteAsync(questionKey, request));
    }
}
