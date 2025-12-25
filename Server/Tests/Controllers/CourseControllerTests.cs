using Core.DTOs;
using Core.Interfaces.Services;
using Core.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Server.Tests.Controllers;

[TestFixture]
public class CourseControllerTests
{
    private Mock<ICourseService> _courseServiceMock;
    private CourseController _courseController;

    [SetUp]
    public void Setup()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _courseController = new CourseController(_courseServiceMock.Object);
    }

    #region GetAll Tests
    [Test]
    public async Task GetAll_WhenServiceReturnsSuccess_ReturnsOkResponse()
    {
        // Arrange
        var courses = new List<CourseDto>
        {
            new CourseDto { Id = 1, Name = "Course 1", Description = "Description 1" },
            new CourseDto { Id = 2, Name = "Course 2", Description = "Description 2" }
        };

        var response = Response<List<CourseDto>>.Ok(courses);

        _courseServiceMock
            .Setup(s => s.GetAllCourses())
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        _courseServiceMock.Verify(s => s.GetAllCourses(), Times.Once);
    }

    [Test]
    public async Task GetAll_WhenServiceReturnsEmptyList_ReturnsOkResponse()
    {
        // Arrange
        var response = Response<List<CourseDto>>.Ok(new List<CourseDto>());

        _courseServiceMock
            .Setup(s => s.GetAllCourses())
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        _courseServiceMock.Verify(s => s.GetAllCourses(), Times.Once);
    }

    [Test]
    public async Task GetAll_WhenServiceReturnsFail_ReturnsErrorResponse()
    {
        // Arrange
        var response = new Response<List<CourseDto>>
        {
            Success = false,
            Message = "Failed to fetch courses"
        };

        _courseServiceMock
            .Setup(s => s.GetAllCourses())
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.GetAll();

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        _courseServiceMock.Verify(s => s.GetAllCourses(), Times.Once);
    }
    #endregion
    
    #region Get Tests
    [Test]
    public async Task Get_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var courseDto = new CourseDto { Id = courseId, Name = "Test Course", Description = "Test Description" };
        var response = Response<CourseDto>.Ok(courseDto);

        _courseServiceMock
            .Setup(s => s.GetCourseById(courseId))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Get(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        _courseServiceMock.Verify(s => s.GetCourseById(courseId), Times.Once);
    }

    [Test]
    public async Task Get_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        var courseId = 999;
        var response = new Response<CourseDto>
        {
            Success = false,
            Message = "Course not found"
        };

        _courseServiceMock
            .Setup(s => s.GetCourseById(courseId))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Get(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        _courseServiceMock.Verify(s => s.GetCourseById(courseId), Times.Once);
    }

    [Test]
    public async Task Get_WhenServiceThrowsException_ReturnsErrorResponse()
    {
        // Arrange
        var courseId = 1;
        var response = new Response<CourseDto>
        {
            Success = false,
            Message = "An unexpected error occurred while fetching the course"
        };

        _courseServiceMock
            .Setup(s => s.GetCourseById(courseId))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Get(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        _courseServiceMock.Verify(s => s.GetCourseById(courseId), Times.Once);
    }
    #endregion
    
    #region Create Tests
    [Test]
    public async Task Create_WithValidInput_ReturnsCreatedResponse()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Name = "Test Course",
            Description = "Test Description"
        };

        var courseDto = new CourseDto
        {
            Id = 1,
            Name = createCourseDto.Name,
            Description = createCourseDto.Description
        };

        var response = new Response<CourseDto>
        {
            Success = true,
            Result = courseDto,
            Message = "Course created successfully"
        };

        _courseServiceMock
            .Setup(s => s.CreateCourse(It.IsAny<CreateCourseDto>()))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Create(createCourseDto);

        // Assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = result as CreatedAtActionResult;
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
        Assert.That(createdResult.Value, Is.EqualTo(courseDto));
        _courseServiceMock.Verify(s => s.CreateCourse(createCourseDto), Times.Once);
    }

    [Test]
    public void Create_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Name = "Test Course",
            Description = "Test Description"
        };

        _courseServiceMock
            .Setup(s => s.CreateCourse(It.IsAny<CreateCourseDto>()))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _courseController.Create(createCourseDto));
        _courseServiceMock.Verify(s => s.CreateCourse(createCourseDto), Times.Once);
    }

    [Test]
    public async Task Create_WhenServiceReturnsFail_ReturnsObjectResult()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Name = "Test Course",
            Description = "Test Description"
        };

        var response = new Response<CourseDto>
        {
            Success = false,
            Message = "Failed to create course"
        };

        _courseServiceMock
            .Setup(s => s.CreateCourse(It.IsAny<CreateCourseDto>()))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Create(createCourseDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        _courseServiceMock.Verify(s => s.CreateCourse(createCourseDto), Times.Once);
    }
    #endregion
    
    #region Update Tests
    [Test]
    public async Task Update_WithValidIdAndData_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var updateDto = new UpdateCourseDto { Name = "Updated Course", Description = "Updated Description" };
        var courseDto = new CourseDto { Id = courseId, Name = "Updated Course", Description = "Updated Description" };
        var response = Response<CourseDto>.Ok(courseDto);

        _courseServiceMock
            .Setup(s => s.UpdateCourse(courseId, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Update(courseId, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        _courseServiceMock.Verify(s => s.UpdateCourse(courseId, updateDto), Times.Once);
    }

    [Test]
    public async Task Update_WithInvalidId_ReturnsNotFoundResponse()
    {
        // Arrange
        var courseId = 999;
        var updateDto = new UpdateCourseDto { Name = "Updated Course", Description = "Updated Description" };
        var response = new Response<CourseDto>
        {
            Success = false,
            Message = "Course not found"
        };

        _courseServiceMock
            .Setup(s => s.UpdateCourse(courseId, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Update(courseId, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        _courseServiceMock.Verify(s => s.UpdateCourse(courseId, updateDto), Times.Once);
    }

    [Test]
    public async Task Update_WhenServiceThrowsException_ReturnsErrorResponse()
    {
        // Arrange
        var courseId = 1;
        var updateDto = new UpdateCourseDto { Name = "Updated Course", Description = "Updated Description" };
        var response = new Response<CourseDto>
        {
            Success = false,
            Message = "An unexpected error occurred while updating the course"
        };

        _courseServiceMock
            .Setup(s => s.UpdateCourse(courseId, updateDto))
            .ReturnsAsync(response);

        // Act
        var result = await _courseController.Update(courseId, updateDto);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        _courseServiceMock.Verify(s => s.UpdateCourse(courseId, updateDto), Times.Once);
    }
    #endregion
    
    #region Delete Tests
    [Test]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var courseId = 1;
        var deleteResponse = Response<bool>.Ok(true);

        _courseServiceMock
            .Setup(s => s.DeleteCourse(courseId))
            .ReturnsAsync(deleteResponse);

        // Act
        var result = await _courseController.Delete(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
        _courseServiceMock.Verify(s => s.DeleteCourse(courseId), Times.Once);
    }

    [Test]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var courseId = 999;
        var deleteResponse = Response<bool>.NotFound("Course not found");

        _courseServiceMock
            .Setup(s => s.DeleteCourse(courseId))
            .ReturnsAsync(deleteResponse);

        // Act
        var result = await _courseController.Delete(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        _courseServiceMock.Verify(s => s.DeleteCourse(courseId), Times.Once);
    }

    [Test]
    public async Task Delete_WhenServiceFails_ReturnsObjectResult()
    {
        // Arrange
        var courseId = 1;
        var deleteResponse = Response<bool>.Fail("An unexpected error occurred while deleting the course");

        _courseServiceMock
            .Setup(s => s.DeleteCourse(courseId))
            .ReturnsAsync(deleteResponse);

        // Act
        var result = await _courseController.Delete(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        _courseServiceMock.Verify(s => s.DeleteCourse(courseId), Times.Once);
    }
    #endregion
}