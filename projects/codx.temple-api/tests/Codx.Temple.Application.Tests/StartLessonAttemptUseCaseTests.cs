using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Moq;

namespace Codx.Temple.Application.Tests;

public class StartLessonAttemptUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<ICurrentUserAccessor> _currentUserMock;
    private readonly StartLessonAttemptUseCase _useCase;

    public StartLessonAttemptUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _currentUserMock = new Mock<ICurrentUserAccessor>();
        _currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _useCase = new StartLessonAttemptUseCase(_dbMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateAttempt()
    {
        var lessonKey = Guid.NewGuid();
        var versionId = Guid.NewGuid();
        var lesson = Lesson.Create(1, "Test");
        typeof(Lesson).GetProperty(nameof(Lesson.Key))!.SetValue(lesson, lessonKey);
        typeof(Lesson).GetProperty(nameof(Lesson.CurrentPublishedVersionId))!.SetValue(lesson, versionId);

        var lessons = new List<Lesson> { lesson };
        var attempts = new List<LessonAttempt>();
        var flags = new List<AnswerFlag>();
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);
        _dbMock.Setup(db => db.LessonAttempts).Returns(DbSetMockHelper.CreateMockDbSet(attempts).Object);
        _dbMock.Setup(db => db.AnswerFlags).Returns(DbSetMockHelper.CreateMockDbSet(flags).Object);

        var result = await _useCase.ExecuteAsync(lessonKey);

        Assert.NotNull(result);
        Assert.Equal(lessonKey, result.LessonKey);
        Assert.Equal(versionId, result.LessonVersionId);
        Assert.Equal("InProgress", result.Status);
    }
}
