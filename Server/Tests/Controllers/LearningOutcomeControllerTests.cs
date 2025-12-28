using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Server.Tests.Controllers;

[TestFixture]
public class LearningOutcomeControllerTests
{
    private Mock<ILearningOutcomeService> _learningOutcomeServiceMock;
    private LearningOutcomeController _controller;

    [SetUp]
    public void Setup()
    {
        _learningOutcomeServiceMock = new Mock<ILearningOutcomeService>();
        _controller = new LearningOutcomeController(_learningOutcomeServiceMock.Object);
    }

    #region GetAll Tests

    [Test]
    public async Task GetAll_WithValidData_ReturnsOkResponse()
    {
        // Arrange
        var learningOutcomes = new List<LearningOutcomeDto>
        {
            new LearningOutcomeDto { Id = 1, Name = "Outcome 1", Description = "Description 1", EndQualification = "Qualification 1", CourseId = 1 },
            new LearningOutcomeDto { Id = 2, Name = "Outcome 2", Description = "Description 2", EndQualification = "Qualification 2", CourseId = 1 }
        };

        var response = Response<List<LearningOutcomeDto>>.Ok(learningOutcomes);

        _learningOutcomeServiceMock
            .Setup(s => s.GetAllLearningOutcomes())
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        _learningOutcomeServiceMock.Verify(s => s.GetAllLearningOutcomes(), Times.Once);
    }

    [Test]
    public async Task GetAll_WithNoData_ReturnsOkResponseWithEmptyList()
    {
        // Arrange
        var response = Response<List<LearningOutcomeDto>>.Ok(new List<LearningOutcomeDto>());

        _learningOutcomeServiceMock
            .Setup(s => s.GetAllLearningOutcomes())
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        _learningOutcomeServiceMock.Verify(s => s.GetAllLearningOutcomes(), Times.Once);
    }

    [Test]
    public async Task GetAll_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var response = Response<List<LearningOutcomeDto>>.Fail("An error occurred");

        _learningOutcomeServiceMock
            .Setup(s => s.GetAllLearningOutcomes())
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        _learningOutcomeServiceMock.Verify(s => s.GetAllLearningOutcomes(), Times.Once);
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_WithValidData_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var createDto = new CreateLearningOutcomeDto
        {
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        var createdDto = new LearningOutcomeDto
        {
            Id = 1,
            Name = createDto.Name,
            Description = createDto.Description,
            EndQualification = createDto.EndQualification,
            CourseId = createDto.CourseId
        };

        var response = Response<LearningOutcomeDto>.Ok(createdDto);

        _learningOutcomeServiceMock
            .Setup(s => s.CreateLearningOutcome(createDto))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = result as CreatedAtActionResult;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        _learningOutcomeServiceMock.Verify(s => s.CreateLearningOutcome(createDto), Times.Once);
    }

    [Test]
    public async Task Create_WhenCourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var createDto = new CreateLearningOutcomeDto
        {
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 999
        };

        var response = Response<LearningOutcomeDto>.NotFound("Course not found");

        _learningOutcomeServiceMock
            .Setup(s => s.CreateLearningOutcome(createDto))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        _learningOutcomeServiceMock.Verify(s => s.CreateLearningOutcome(createDto), Times.Once);
    }

    [Test]
    public async Task Create_WhenServiceFails_ReturnsBadRequest(){
        // Arrange
        var createDto = new CreateLearningOutcomeDto
        {
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        var response = Response<LearningOutcomeDto>.Fail("An error occurred");

        _learningOutcomeServiceMock
            .Setup(s => s.CreateLearningOutcome(createDto))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        _learningOutcomeServiceMock.Verify(s => s.CreateLearningOutcome(createDto), Times.Once);
    }
    #endregion
}