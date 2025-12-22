using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;

namespace Core.Tests.Services;

[TestFixture]
public class CourseServiceTests
{
    private Mock<IRepository<Course>> _courseRepositoryMock;
    private CourseService _courseService;

    [SetUp]
    public void Setup()
    {
        _courseRepositoryMock = new Mock<IRepository<Course>>();
        _courseService = new CourseService(_courseRepositoryMock.Object);
    }

    [Test]
    public async Task CreateCourse_WithValidInput_ReturnsSuccessResponse()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Name = "Test Course",
            Description = "Test Description"
        };

        var course = new Course
        {
            Id = 1,
            Name = createCourseDto.Name,
            Description = createCourseDto.Description
        };

        _courseRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Course>()))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.CreateCourse(createCourseDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Id, Is.EqualTo(course.Id));
        Assert.That(result.Result.Name, Is.EqualTo(createCourseDto.Name));
        Assert.That(result.Result.Description, Is.EqualTo(createCourseDto.Description));
        _courseRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<Course>()), Times.Once);
    }

    [Test]
    public async Task CreateCourse_WhenRepositoryThrowsInvalidOperation_ReturnsFailResponse()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Name = "Test Course",
            Description = "Test Description"
        };

        _courseRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Course>()))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await _courseService.CreateCourse(createCourseDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Invalid operation while updating course"));
    }

    [Test]
    public async Task CreateCourse_WhenRepositoryThrowsException_ReturnsFailResponse()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Name = "Test Course",
            Description = "Test Description"
        };

        _courseRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Course>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _courseService.CreateCourse(createCourseDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("An unexpected error occurred while updating the course"));
    }
}