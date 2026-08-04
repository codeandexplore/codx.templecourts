using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Moq;

namespace Codx.Temple.Application.Tests;

public class ListStudentLessonsUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly ListStudentLessonsUseCase _useCase;

    public ListStudentLessonsUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _useCase = new ListStudentLessonsUseCase(_dbMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnPublishedOnly()
    {
        var published = Lesson.Create(1, "Published");
        typeof(Lesson).GetProperty(nameof(Lesson.CurrentPublishedVersionId))!.SetValue(published, Guid.NewGuid());
        var draft = Lesson.Create(2, "Draft");
        var archived = Lesson.Create(3, "Archived");
        archived.Archive();

        var lessons = new List<Lesson> { published, draft, archived };
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);

        var result = await _useCase.ExecuteAsync();

        Assert.Single(result);
        Assert.Equal("Published", result[0].Title);
    }
}
