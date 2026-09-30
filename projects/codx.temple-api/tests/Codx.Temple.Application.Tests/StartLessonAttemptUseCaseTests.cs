using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
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
    private readonly Guid _studentId;
    private readonly Guid _lessonKey;

    public StartLessonAttemptUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _currentUserMock = new Mock<ICurrentUserAccessor>();
        _studentId = Guid.NewGuid();
        _lessonKey = Guid.NewGuid();
        _currentUserMock.Setup(x => x.UserId).Returns(_studentId);
        _useCase = new StartLessonAttemptUseCase(_dbMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_UnresolvedFlag_ShouldBlockNewAttempt()
    {
        var lessons = new List<Lesson>();
        var attempts = new List<LessonAttempt>().AsQueryable();
        var flags = new List<AnswerFlag> { AnswerFlag.Create(_studentId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()) };

        _dbMock.Setup(db => db.LessonAttempts).Returns(DbSetMockHelper.CreateMockDbSet(attempts).Object);
        _dbMock.Setup(db => db.AnswerFlags).Returns(DbSetMockHelper.CreateMockDbSet(flags).Object);
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);

        await Assert.ThrowsAsync<GatingBlockedException>(() => _useCase.ExecuteAsync(_lessonKey));
    }

    [Fact]
    public async Task ExecuteAsync_NoFlag_ShouldCreateAttempt()
    {
        var lesson = Lesson.Create(1, "Lesson One");
        typeof(Lesson).GetProperty(nameof(Lesson.Key))!.SetValue(lesson, _lessonKey);
        typeof(Lesson).GetProperty(nameof(Lesson.CurrentPublishedVersionId))!.SetValue(lesson, Guid.NewGuid());
        var lessons = new List<Lesson> { lesson };
        var attempts = new List<LessonAttempt>().AsQueryable();
        var flags = new List<AnswerFlag>().AsQueryable();

        _dbMock.Setup(db => db.LessonAttempts).Returns(DbSetMockHelper.CreateMockDbSet(attempts).Object);
        _dbMock.Setup(db => db.AnswerFlags).Returns(DbSetMockHelper.CreateMockDbSet(flags).Object);
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);

        var result = await _useCase.ExecuteAsync(_lessonKey);

        Assert.NotNull(result);
        Assert.Equal("InProgress", result.Status);
        Assert.Equal(_lessonKey, result.LessonKey);
    }
}
