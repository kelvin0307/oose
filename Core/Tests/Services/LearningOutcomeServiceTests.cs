using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;

namespace Core.Tests.Services;

[TestFixture]
public class LearningOutcomeServiceTests
{
    private Mock<IRepository<LearningOutcome>> learningOutcomeRepositoryMock;
    private Mock<IRepository<Course>> courseRepositoryMock;
    private ILearningOutcomeService learningOutcomeService;

    [SetUp]
    public void Setup()
    {
        learningOutcomeRepositoryMock = new Mock<IRepository<LearningOutcome>>();
        courseRepositoryMock = new Mock<IRepository<Course>>();
        learningOutcomeService = new LearningOutcomeService(learningOutcomeRepositoryMock.Object, courseRepositoryMock.Object);
    }

    #region GetAllLearningOutcomes Tests
    [Test]
    public async Task GetAllLearningOutcomes_WithValidData_ReturnsOkResponseWithLearningOutcomes()
    {
        // Arrange
        var learningOutcomes = new List<LearningOutcome>
        {
            new LearningOutcome { Id = 1, Name = "Outcome 1", Description = "Description 1", EndQualification = "Qualification 1", CourseId = 1 },
            new LearningOutcome { Id = 2, Name = "Outcome 2", Description = "Description 2", EndQualification = "Qualification 2", CourseId = 1 }
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(learningOutcomes);

        // Act
        var result = await learningOutcomeService.GetAllLearningOutcomes();

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Has.Count.EqualTo(2));
        Assert.That(result.Result[0].Name, Is.EqualTo("Outcome 1"));
        Assert.That(result.Result[1].Name, Is.EqualTo("Outcome 2"));
        learningOutcomeRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllLearningOutcomes_WithNoData_ReturnsOkResponseWithEmptyList()
    {
        // Arrange
        learningOutcomeRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<LearningOutcome>());

        // Act
        var result = await learningOutcomeService.GetAllLearningOutcomes();

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Has.Count.EqualTo(0));
        learningOutcomeRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllLearningOutcomes_WhenInvalidOperationThrown_ReturnsFailResponseWithInvalidOperation()
    {
        // Arrange
        learningOutcomeRepositoryMock
            .Setup(r => r.GetAll())
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await learningOutcomeService.GetAllLearningOutcomes();

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while fetching course"));
        learningOutcomeRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllLearningOutcomes_WhenExceptionThrown_ReturnsFailResponse()
    {
        // Arrange
        learningOutcomeRepositoryMock
            .Setup(r => r.GetAll())
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await learningOutcomeService.GetAllLearningOutcomes();

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching courses"));
        learningOutcomeRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }
    #endregion
    
    #region CreateLearningOutcome Tests

    [Test]
    public async Task CreateLearningOutcome_WithValidData_ReturnsOkResponse()
    {
        // Arrange
        var createDto = new CreateLearningOutcomeDto
        {
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        var course = new Course { Id = 1, Name = "Test Course" };
        var createdLearningOutcome = new LearningOutcome
        {
            Id = 1,
            Name = createDto.Name,
            Description = createDto.Description,
            EndQualification = createDto.EndQualification,
            CourseId = createDto.CourseId
        };

        courseRepositoryMock
            .Setup(r => r.Get(createDto.CourseId))
            .ReturnsAsync(course);

        learningOutcomeRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()))
            .ReturnsAsync(createdLearningOutcome);

        // Act
        var result = await learningOutcomeService.CreateLearningOutcome(createDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Name, Is.EqualTo("Test Outcome"));
        Assert.That(result.Result.CourseId, Is.EqualTo(1));
        courseRepositoryMock.Verify(r => r.Get(createDto.CourseId), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    [Test]
    public async Task CreateLearningOutcome_WhenCourseNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        var createDto = new CreateLearningOutcomeDto
        {
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 999
        };

        courseRepositoryMock
            .Setup(r => r.Get(createDto.CourseId))
            .ReturnsAsync((Course)null);

        // Act
        var result = await learningOutcomeService.CreateLearningOutcome(createDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Course not found"));
        courseRepositoryMock.Verify(r => r.Get(createDto.CourseId), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()), Times.Never);
    }

    [Test]
    public async Task CreateLearningOutcome_WhenInvalidOperationThrown_ReturnsFailResponse()
    {
        // Arrange
        var createDto = new CreateLearningOutcomeDto
        {
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        var course = new Course { Id = 1, Name = "Test Course" };

        courseRepositoryMock
            .Setup(r => r.Get(createDto.CourseId))
            .ReturnsAsync(course);

        learningOutcomeRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await learningOutcomeService.CreateLearningOutcome(createDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while updating course"));
        learningOutcomeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    [Test]
    public async Task CreateLearningOutcome_WhenExceptionThrown_ReturnsFailResponse()
    {
        // Arrange
        var createDto = new CreateLearningOutcomeDto
        {
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        var course = new Course { Id = 1, Name = "Test Course" };

        courseRepositoryMock
            .Setup(r => r.Get(createDto.CourseId))
            .ReturnsAsync(course);

        learningOutcomeRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await learningOutcomeService.CreateLearningOutcome(createDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while updating the course"));
        learningOutcomeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    #endregion
}