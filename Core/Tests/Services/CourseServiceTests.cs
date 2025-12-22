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
    public async Task GetAllCourses_WithValidCourses_ReturnsOkResponse()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1", Description = "Description 1" },
            new Course { Id = 2, Name = "Course 2", Description = "Description 2" }
        };

        _courseRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Has.Count.EqualTo(2));
        Assert.That(result.Result[0].Name, Is.EqualTo("Course 1"));
        Assert.That(result.Result[1].Name, Is.EqualTo("Course 2"));
        _courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllCourses_WithNoCourses_ReturnsEmptyList()
    {
        // Arrange
        var courses = new List<Course>();

        _courseRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Empty);
        _courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllCourses_WhenInvalidOperationExceptionThrown_ReturnsFail()
    {
        // Arrange
        _courseRepositoryMock
            .Setup(r => r.GetAll())
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await _courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while fetching course"));
        _courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllCourses_WhenGeneralExceptionThrown_ReturnsFail()
    {
        // Arrange
        _courseRepositoryMock
            .Setup(r => r.GetAll())
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching courses"));
        _courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
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