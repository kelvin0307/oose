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
        Assert.That(result.Message, Does.Contain("Invalid operation while fetching learning outcomes"));
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
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching learning outcomes"));
        learningOutcomeRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }
    #endregion
    
    #region GetLearningOutcomeById Tests

    [Test]
    public async Task GetLearningOutcomeById_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var learningOutcome = new LearningOutcome
        {
            Id = 1,
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(1))
            .ReturnsAsync(learningOutcome);

        // Act
        var result = await learningOutcomeService.GetLearningOutcomeById(1);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Id, Is.EqualTo(1));
        Assert.That(result.Result.Name, Is.EqualTo("Test Outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(1), Times.Once);
    }

    [Test]
    public async Task GetLearningOutcomeById_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        learningOutcomeRepositoryMock
            .Setup(r => r.Get(999))
            .ReturnsAsync((LearningOutcome)null);

        // Act
        var result = await learningOutcomeService.GetLearningOutcomeById(999);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Learning outcome not found"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(999), Times.Once);
    }

    [Test]
    public async Task GetLearningOutcomeById_WhenInvalidOperationThrown_ReturnsFailResponse()
    {
        // Arrange
        learningOutcomeRepositoryMock
            .Setup(r => r.Get(1))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await learningOutcomeService.GetLearningOutcomeById(1);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while getting learning outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(1), Times.Once);
    }

    [Test]
    public async Task GetLearningOutcomeById_WhenExceptionThrown_ReturnsFailResponse()
    {
        // Arrange
        learningOutcomeRepositoryMock
            .Setup(r => r.Get(1))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await learningOutcomeService.GetLearningOutcomeById(1);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching the learning outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(1), Times.Once);
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
        Assert.That(result.Message, Does.Contain("Invalid operation while creating learning outcome"));
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
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while creating the learning outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    #endregion
    
    #region UpdateLearningOutcome Tests

    [Test]
    public async Task UpdateLearningOutcome_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateLearningOutcomeDto
        {
            Name = "Updated Outcome",
            Description = "Updated Description",
            EndQualification = "Updated Qualification"
        };

        var existingLearningOutcome = new LearningOutcome
        {
            Id = id,
            Name = "Old Outcome",
            Description = "Old Description",
            EndQualification = "Old Qualification",
            CourseId = 1
        };

        var updatedLearningOutcome = new LearningOutcome
        {
            Id = id,
            Name = updateDto.Name,
            Description = updateDto.Description,
            EndQualification = updateDto.EndQualification,
            CourseId = 1
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync(existingLearningOutcome);

        learningOutcomeRepositoryMock
            .Setup(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()))
            .ReturnsAsync(updatedLearningOutcome);

        // Act
        var result = await learningOutcomeService.UpdateLearningOutcome(id, updateDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Id, Is.EqualTo(id));
        Assert.That(result.Result.Name, Is.EqualTo("Updated Outcome"));
        Assert.That(result.Result.Description, Is.EqualTo("Updated Description"));
        Assert.That(result.Result.EndQualification, Is.EqualTo("Updated Qualification"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    [Test]
    public async Task UpdateLearningOutcome_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        var id = 999;
        var updateDto = new UpdateLearningOutcomeDto
        {
            Name = "Updated Outcome",
            Description = "Updated Description",
            EndQualification = "Updated Qualification"
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync((LearningOutcome)null);

        // Act
        var result = await learningOutcomeService.UpdateLearningOutcome(id, updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Learning outcome not found"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Never);
    }

    [Test]
    public async Task UpdateLearningOutcome_WhenInvalidOperationThrown_ReturnsFailResponse()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateLearningOutcomeDto
        {
            Name = "Updated Outcome",
            Description = "Updated Description",
            EndQualification = "Updated Qualification"
        };

        var existingLearningOutcome = new LearningOutcome
        {
            Id = id,
            Name = "Old Outcome",
            Description = "Old Description",
            EndQualification = "Old Qualification",
            CourseId = 1
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync(existingLearningOutcome);

        learningOutcomeRepositoryMock
            .Setup(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await learningOutcomeService.UpdateLearningOutcome(id, updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while updating learning outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    [Test]
    public async Task UpdateLearningOutcome_WhenExceptionThrown_ReturnsFailResponse()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateLearningOutcomeDto
        {
            Name = "Updated Outcome",
            Description = "Updated Description",
            EndQualification = "Updated Qualification"
        };

        var existingLearningOutcome = new LearningOutcome
        {
            Id = id,
            Name = "Old Outcome",
            Description = "Old Description",
            EndQualification = "Old Qualification",
            CourseId = 1
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync(existingLearningOutcome);

        learningOutcomeRepositoryMock
            .Setup(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await learningOutcomeService.UpdateLearningOutcome(id, updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while updating the learning outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    #endregion
    
    #region DeleteLearningOutcome Tests

    [Test]
    public async Task DeleteLearningOutcome_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var id = 1;
        var learningOutcome = new LearningOutcome
        {
            Id = id,
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync(learningOutcome);

        learningOutcomeRepositoryMock
            .Setup(r => r.DeleteAndCommit(id))
            .ReturnsAsync(learningOutcome);

        // Act
        var result = await learningOutcomeService.DeleteLearningOutcome(id);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.True);
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.DeleteAndCommit(id), Times.Once);
    }

    [Test]
    public async Task DeleteLearningOutcome_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        var id = 999;

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync((LearningOutcome)null);

        // Act
        var result = await learningOutcomeService.DeleteLearningOutcome(id);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Learning outcome not found"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.DeleteAndCommit(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task DeleteLearningOutcome_WhenInvalidOperationThrown_ReturnsFailResponse()
    {
        // Arrange
        var id = 1;
        var learningOutcome = new LearningOutcome
        {
            Id = id,
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync(learningOutcome);

        learningOutcomeRepositoryMock
            .Setup(r => r.DeleteAndCommit(id))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await learningOutcomeService.DeleteLearningOutcome(id);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while deleting learning outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.DeleteAndCommit(id), Times.Once);
    }

    [Test]
    public async Task DeleteLearningOutcome_WhenExceptionThrown_ReturnsFailResponse()
    {
        // Arrange
        var id = 1;
        var learningOutcome = new LearningOutcome
        {
            Id = id,
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(id))
            .ReturnsAsync(learningOutcome);

        learningOutcomeRepositoryMock
            .Setup(r => r.DeleteAndCommit(id))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await learningOutcomeService.DeleteLearningOutcome(id);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while deleting the learning outcome"));
        learningOutcomeRepositoryMock.Verify(r => r.Get(id), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.DeleteAndCommit(id), Times.Once);
    }

    #endregion
}