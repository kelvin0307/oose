using Core.DTOs;
using Core.Interfaces.Services;
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
}