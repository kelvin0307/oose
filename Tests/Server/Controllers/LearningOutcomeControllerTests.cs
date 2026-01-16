using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Tests.Server.Controllers;

[TestFixture]
public class LearningOutcomeControllerTests
{
    private Mock<ILearningOutcomeService> learningOutcomeServiceMock;
    private Mock<IRubricService> rubricServiceMock;
    private LearningOutcomeController controller;

    [SetUp]
    public void Setup()
    {
        learningOutcomeServiceMock = new Mock<ILearningOutcomeService>();
        rubricServiceMock = new Mock<IRubricService>();
        controller = new LearningOutcomeController(learningOutcomeServiceMock.Object, rubricServiceMock.Object);
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

        learningOutcomeServiceMock
            .Setup(s => s.GetAllLearningOutcomes())
            .ReturnsAsync(response);

        // Act
        var result = await controller.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        learningOutcomeServiceMock.Verify(s => s.GetAllLearningOutcomes(), Times.Once);
    }

    [Test]
    public async Task GetAll_WithNoData_ReturnsOkResponseWithEmptyList()
    {
        // Arrange
        var response = Response<List<LearningOutcomeDto>>.Ok(new List<LearningOutcomeDto>());

        learningOutcomeServiceMock
            .Setup(s => s.GetAllLearningOutcomes())
            .ReturnsAsync(response);

        // Act
        var result = await controller.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        learningOutcomeServiceMock.Verify(s => s.GetAllLearningOutcomes(), Times.Once);
    }

    [Test]
    public async Task GetAll_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var response = Response<List<LearningOutcomeDto>>.Fail("An error occurred");

        learningOutcomeServiceMock
            .Setup(s => s.GetAllLearningOutcomes())
            .ReturnsAsync(response);

        // Act
        var result = await controller.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        learningOutcomeServiceMock.Verify(s => s.GetAllLearningOutcomes(), Times.Once);
    }

    #endregion
    
    #region Get Tests

    [Test]
    public async Task Get_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var learningOutcomeDto = new LearningOutcomeDto
        {
            Id = 1,
            Name = "Test Outcome",
            Description = "Test Description",
            EndQualification = "Test Qualification",
            CourseId = 1
        };

        var response = Response<LearningOutcomeDto>.Ok(learningOutcomeDto);

        learningOutcomeServiceMock
            .Setup(s => s.GetLearningOutcomeById(1))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Get(1);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        learningOutcomeServiceMock.Verify(s => s.GetLearningOutcomeById(1), Times.Once);
    }

    [Test]
    public async Task Get_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        var response = Response<LearningOutcomeDto>.NotFound("Learning outcome not found");

        learningOutcomeServiceMock
            .Setup(s => s.GetLearningOutcomeById(999))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Get(999);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        learningOutcomeServiceMock.Verify(s => s.GetLearningOutcomeById(999), Times.Once);
    }

    [Test]
    public async Task Get_WhenServiceFails_ReturnsObjectResultWithBadRequest()
    {
        // Arrange
        var response = Response<LearningOutcomeDto>.Fail("An error occurred");

        learningOutcomeServiceMock
            .Setup(s => s.GetLearningOutcomeById(1))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Get(1);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        learningOutcomeServiceMock.Verify(s => s.GetLearningOutcomeById(1), Times.Once);
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

        learningOutcomeServiceMock
            .Setup(s => s.CreateLearningOutcome(createDto))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = result as CreatedAtActionResult;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        learningOutcomeServiceMock.Verify(s => s.CreateLearningOutcome(createDto), Times.Once);
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

        learningOutcomeServiceMock
            .Setup(s => s.CreateLearningOutcome(createDto))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        learningOutcomeServiceMock.Verify(s => s.CreateLearningOutcome(createDto), Times.Once);
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

        learningOutcomeServiceMock
            .Setup(s => s.CreateLearningOutcome(createDto))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        learningOutcomeServiceMock.Verify(s => s.CreateLearningOutcome(createDto), Times.Once);
    }
    #endregion
    
    #region Update Tests

    [Test]
    public async Task Update_WithValidIdAndDto_ReturnsOkResponse()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateLearningOutcomeDto
        {
            Name = "Updated Outcome",
            Description = "Updated Description",
            EndQualification = "Updated Qualification"
        };

        var learningOutcomeDto = new LearningOutcomeDto
        {
            Id = id,
            Name = updateDto.Name,
            Description = updateDto.Description,
            EndQualification = updateDto.EndQualification,
            CourseId = 1
        };

        var response = Response<LearningOutcomeDto>.Ok(learningOutcomeDto);

        learningOutcomeServiceMock
            .Setup(s => s.UpdateLearningOutcome(id, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Update(id, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        learningOutcomeServiceMock.Verify(s => s.UpdateLearningOutcome(id, updateDto), Times.Once);
    }

    [Test]
    public async Task Update_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        var id = 999;
        var updateDto = new UpdateLearningOutcomeDto
        {
            Name = "Updated Outcome",
            Description = "Updated Description",
            EndQualification = "Updated Qualification"
        };

        var response = Response<LearningOutcomeDto>.NotFound("Learning outcome not found");

        learningOutcomeServiceMock
            .Setup(s => s.UpdateLearningOutcome(id, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Update(id, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        learningOutcomeServiceMock.Verify(s => s.UpdateLearningOutcome(id, updateDto), Times.Once);
    }

    [Test]
    public async Task Update_WhenServiceFails_ReturnsObjectResultWithInternalServerError()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateLearningOutcomeDto
        {
            Name = "Updated Outcome",
            Description = "Updated Description",
            EndQualification = "Updated Qualification"
        };

        var response = Response<LearningOutcomeDto>.Fail("An error occurred");

        learningOutcomeServiceMock
            .Setup(s => s.UpdateLearningOutcome(id, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Update(id, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        learningOutcomeServiceMock.Verify(s => s.UpdateLearningOutcome(id, updateDto), Times.Once);
    }

    #endregion
    
    #region Delete Tests

    [Test]
    public async Task Delete_WithValidId_ReturnsNoContentResponse()
    {
        // Arrange
        var id = 1;
        var response = Response<bool>.Ok(true);

        learningOutcomeServiceMock
            .Setup(s => s.DeleteLearningOutcome(id))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
        learningOutcomeServiceMock.Verify(s => s.DeleteLearningOutcome(id), Times.Once);
    }

    [Test]
    public async Task Delete_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        var id = 999;
        var response = Response<bool>.NotFound("Learning outcome not found");

        learningOutcomeServiceMock
            .Setup(s => s.DeleteLearningOutcome(id))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        learningOutcomeServiceMock.Verify(s => s.DeleteLearningOutcome(id), Times.Once);
    }

    [Test]
    public async Task Delete_WhenServiceFails_ReturnsObjectResultWithInternalServerError()
    {
        // Arrange
        var id = 1;
        var response = Response<bool>.Fail("An error occurred");

        learningOutcomeServiceMock
            .Setup(s => s.DeleteLearningOutcome(id))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        learningOutcomeServiceMock.Verify(s => s.DeleteLearningOutcome(id), Times.Once);
    }

    #endregion
}
