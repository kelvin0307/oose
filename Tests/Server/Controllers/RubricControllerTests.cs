using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Server.Tests.Controllers;

[TestFixture]
public class RubricControllerTests
{
    private Mock<IRubricService> rubricServiceMock;
    private RubricController rubricController;

    [SetUp]
    public void Setup()
    {
        rubricServiceMock = new Mock<IRubricService>();
        rubricController = new RubricController(rubricServiceMock.Object);
    }

    #region GetAll Tests

    [Test]
    public async Task GetAll_WithValidRubrics_ReturnsOkResponse()
    {
        // Arrange
        var rubrics = new List<RubricDto>
        {
            new RubricDto { Id = 1, Name = "Rubric 1" },
            new RubricDto { Id = 2, Name = "Rubric 2" }
        };

        var response = Response<List<RubricDto>>.Ok(rubrics);

        rubricServiceMock
            .Setup(s => s.GetAllRubrics())
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));

        rubricServiceMock.Verify(s => s.GetAllRubrics(), Times.Once);
    }

    [Test]
    public async Task GetAll_WhenServiceReturnsFail_ReturnsObjectResult()
    {
        // Arrange
        var response = new Response<List<RubricDto>>
        {
            Success = false,
            Message = "Failed to fetch rubrics"
        };

        rubricServiceMock
            .Setup(s => s.GetAllRubrics())
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));

        rubricServiceMock.Verify(s => s.GetAllRubrics(), Times.Once);
    }

    #endregion

    #region Get Tests

    [Test]
    public async Task Get_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var rubricId = 1;
        var rubricDto = new RubricDto { Id = rubricId, Name = "Test Rubric" };
        var response = Response<RubricDto>.Ok(rubricDto);

        rubricServiceMock
            .Setup(s => s.GetRubricById(rubricId))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Get(rubricId);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));

        rubricServiceMock.Verify(s => s.GetRubricById(rubricId), Times.Once);
    }

    [Test]
    public async Task Get_WithInvalidId_ReturnsObjectResult()
    {
        // Arrange
        var rubricId = 999;
        var response = new Response<RubricDto>
        {
            Success = false,
            Message = "Rubric not found"
        };

        rubricServiceMock
            .Setup(s => s.GetRubricById(rubricId))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Get(rubricId);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));

        rubricServiceMock.Verify(s => s.GetRubricById(rubricId), Times.Once);
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_WithValidInput_ReturnsCreatedResponse()
    {
        // Arrange
        var createRubricDto = new CreateRubricDto
        {
            Name = "New Rubric",
            LearningOutcomeId = 1
        };

        var rubricDto = new RubricDto
        {
            Id = 1,
            Name = "New Rubric"
        };

        var response = new Response<RubricDto>
        {
            Success = true,
            Result = rubricDto
        };

        rubricServiceMock
            .Setup(s => s.CreateRubric(It.IsAny<CreateRubricDto>()))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Create(createRubricDto);

        // Assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = result as CreatedAtActionResult;
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
        Assert.That(createdResult.Value, Is.EqualTo(rubricDto));

        rubricServiceMock.Verify(s => s.CreateRubric(createRubricDto), Times.Once);
    }

    [Test]
    public async Task Create_WhenServiceReturnsFail_ReturnsObjectResult()
    {
        // Arrange
        var createRubricDto = new CreateRubricDto
        {
            Name = "New Rubric",
            LearningOutcomeId = 1
        };

        var response = new Response<RubricDto>
        {
            Success = false,
            Message = "Failed to create rubric"
        };

        rubricServiceMock
            .Setup(s => s.CreateRubric(It.IsAny<CreateRubricDto>()))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Create(createRubricDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));

        rubricServiceMock.Verify(s => s.CreateRubric(createRubricDto), Times.Once);
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task Update_WithValidInput_ReturnsOkResponse()
    {
        // Arrange
        var rubricId = 1;
        var updateDto = new UpdateRubricDto { Name = "Updated Rubric" };

        var rubricDto = new RubricDto { Id = rubricId, Name = "Updated Rubric" };
        var response = Response<RubricDto>.Ok(rubricDto);

        rubricServiceMock
            .Setup(s => s.UpdateRubric(rubricId, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Update(rubricId, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));

        rubricServiceMock.Verify(s => s.UpdateRubric(rubricId, updateDto), Times.Once);
    }

    [Test]
    public async Task Update_WhenServiceReturnsFail_ReturnsObjectResult()
    {
        // Arrange
        var rubricId = 1;
        var updateDto = new UpdateRubricDto { Name = "Updated Rubric" };

        var response = new Response<RubricDto>
        {
            Success = false,
            Message = "Failed to update rubric"
        };

        rubricServiceMock
            .Setup(s => s.UpdateRubric(rubricId, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Update(rubricId, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));

        rubricServiceMock.Verify(s => s.UpdateRubric(rubricId, updateDto), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var rubricId = 1;
        var response = Response<bool>.Ok(true);

        rubricServiceMock
            .Setup(s => s.DeleteRubric(rubricId))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Delete(rubricId);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
        rubricServiceMock.Verify(s => s.DeleteRubric(rubricId), Times.Once);
    }

    [Test]
    public async Task Delete_WhenServiceReturnsFail_ReturnsObjectResult()
    {
        // Arrange
        var rubricId = 1;
        var response = new Response<bool>
        {
            Success = false,
            Message = "Rubric not found"
        };

        rubricServiceMock
            .Setup(s => s.DeleteRubric(rubricId))
            .ReturnsAsync(response);

        // Act
        var result = await rubricController.Delete(rubricId);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));

        rubricServiceMock.Verify(s => s.DeleteRubric(rubricId), Times.Once);
    }

    #endregion
}