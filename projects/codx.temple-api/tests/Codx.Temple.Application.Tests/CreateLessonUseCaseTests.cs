using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Moq;

namespace Codx.Temple.Application.Tests;

public class CreateLessonUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly CreateLessonUseCase _useCase;

    public CreateLessonUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _useCase = new CreateLessonUseCase(_dbMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateLesson()
    {
        var request = new CreateLessonRequest(1, "Test Lesson");
        var lessons = new List<Lesson>().AsQueryable();
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);

        var result = await _useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.Number);
        Assert.Equal("Test Lesson", result.Title);
        Assert.Equal("Active", result.Status);
        Assert.Null(result.CurrentPublishedVersionId);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateNumber_ShouldThrowConflict()
    {
        var request = new CreateLessonRequest(1, "Another One");
        var existing = Lesson.Create(1, "Existing");
        var lessons = new List<Lesson> { existing }.AsQueryable();
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);

        await Assert.ThrowsAsync<ConflictException>(() => _useCase.ExecuteAsync(request));
    }
}
