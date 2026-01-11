using AutoMapper;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;

namespace Core.Tests.Services;

[TestFixture]
public class CourseServiceTests
{
    private Mock<IRepository<Course>> courseRepositoryMock;
    private Mock<IMapper> mapperMock;
    private ICourseService courseService;

    [SetUp]
    public void Setup()
    {
        courseRepositoryMock = new Mock<IRepository<Course>>();
        mapperMock = new Mock<IMapper>();
        courseService = new CourseService(courseRepositoryMock.Object, mapperMock.Object);
    }

    #region GetAllCourses Tests
    [Test]
    public async Task GetAllCourses_WithValidCourses_ReturnsOkResponse()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1", Description = "Description 1" },
            new Course { Id = 2, Name = "Course 2", Description = "Description 2" }
        };

        var courseDtos = new List<CourseDto>
        {
            new CourseDto { Id = 1, Name = "Course 1", Description = "Description 1" },
            new CourseDto { Id = 2, Name = "Course 2", Description = "Description 2" }
        };

        courseRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(courses);

        mapperMock.Setup(m => m.Map<CourseDto>(courses[0])).Returns(courseDtos[0]);
        mapperMock.Setup(m => m.Map<CourseDto>(courses[1])).Returns(courseDtos[1]);

        // Act
        var result = await courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Has.Count.EqualTo(2));
        Assert.That(result.Result[0].Name, Is.EqualTo("Course 1"));
        Assert.That(result.Result[1].Name, Is.EqualTo("Course 2"));
        courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        mapperMock.Verify(m => m.Map<CourseDto>(It.IsAny<Course>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetAllCourses_WithNoCourses_ReturnsEmptyList()
    {
        // Arrange
        var courses = new List<Course>();

        courseRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(courses);

        // Act
        var result = await courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Empty);
        courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllCourses_WhenInvalidOperationExceptionThrown_ReturnsFail()
    {
        // Arrange
        courseRepositoryMock
            .Setup(r => r.GetAll())
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while fetching course"));
        courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAllCourses_WhenGeneralExceptionThrown_ReturnsFail()
    {
        // Arrange
        courseRepositoryMock
            .Setup(r => r.GetAll())
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await courseService.GetAllCourses();

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching courses"));
        courseRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }
    #endregion

    #region GetCourseById Tests
    [Test]
    public async Task GetCourseById_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Name = "Test Course", Description = "Test Description" };
        var courseDto = new CourseDto { Id = courseId, Name = "Test Course", Description = "Test Description" };

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync(course);

        mapperMock.Setup(m => m.Map<CourseDto>(course)).Returns(courseDto);

        // Act
        var result = await courseService.GetCourseById(courseId);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Id, Is.EqualTo(courseId));
        Assert.That(result.Result.Name, Is.EqualTo("Test Course"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
        mapperMock.Verify(m => m.Map<CourseDto>(course), Times.Once);
    }

    [Test]
    public async Task GetCourseById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var courseId = 999;

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync((Course)null);

        // Act
        var result = await courseService.GetCourseById(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Course not found"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
    }

    [Test]
    public async Task GetCourseById_WhenInvalidOperationExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await courseService.GetCourseById(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while getting course"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
    }

    [Test]
    public async Task GetCourseById_WhenGeneralExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await courseService.GetCourseById(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching the course"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
    }
    #endregion

    #region CreateCourse Tests
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
            Description = createCourseDto.Description,
            Status = Domain.Enums.CourseStatus.Concept
        };

        var courseDto = new CourseDto
        {
            Id = 1,
            Name = "Test Course",
            Description = "Test Description",
            Status = Domain.Enums.CourseStatus.Concept
        };

        courseRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Course>()))
            .ReturnsAsync(course);

        mapperMock.Setup(m => m.Map<CourseDto>(course)).Returns(courseDto);
        mapperMock.Setup(m => m.Map<Course>(createCourseDto)).Returns(course);

        // Act
        var result = await courseService.CreateCourse(createCourseDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Id, Is.EqualTo(course.Id));
        Assert.That(result.Result.Name, Is.EqualTo(createCourseDto.Name));
        Assert.That(result.Result.Description, Is.EqualTo(createCourseDto.Description));
        courseRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<Course>()), Times.Once);
        mapperMock.Verify(m => m.Map<CourseDto>(course), Times.Once);
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

        var course = new Course
        {
            Id = 1,
            Name = createCourseDto.Name,
            Description = createCourseDto.Description,
            Status = Domain.Enums.CourseStatus.Concept
        };

        courseRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Course>()))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        mapperMock.Setup(m => m.Map<Course>(createCourseDto)).Returns(course);


        // Act
        var result = await courseService.CreateCourse(createCourseDto);

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

        courseRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Course>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await courseService.CreateCourse(createCourseDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("An unexpected error occurred while updating the course"));
    }
    #endregion

    #region UpdateCourse Tests
    [Test]
    public async Task UpdateCourse_WithValidIdAndData_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var updateDto = new UpdateCourseDto { Name = "Updated Course", Description = "Updated Description" };
        var existingCourse = new Course { Id = courseId, Name = "Old Course", Description = "Old Description" };
        var updatedCourse = new Course { Id = courseId, Name = "Updated Course", Description = "Updated Description" };
        var courseDto = new CourseDto { Id = courseId, Name = "Updated Course", Description = "Updated Description" };

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync(existingCourse);

        courseRepositoryMock
            .Setup(r => r.UpdateAndCommit(It.IsAny<Course>()))
            .ReturnsAsync(updatedCourse);

        mapperMock.Setup(m => m.Map<CourseDto>(updatedCourse)).Returns(courseDto);

        // Act
        var result = await courseService.UpdateCourse(courseId, updateDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Name, Is.EqualTo("Updated Course"));
        Assert.That(result.Result.Description, Is.EqualTo("Updated Description"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
        courseRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Course>()), Times.Once);
        mapperMock.Verify(m => m.Map<CourseDto>(updatedCourse), Times.Once);
    }

    [Test]
    public async Task UpdateCourse_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var courseId = 999;
        var updateDto = new UpdateCourseDto { Name = "Updated Course", Description = "Updated Description" };

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync((Course)null);

        // Act
        var result = await courseService.UpdateCourse(courseId, updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Course not found"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
        courseRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Course>()), Times.Never);
    }

    [Test]
    public async Task UpdateCourse_WhenInvalidOperationExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;
        var updateDto = new UpdateCourseDto { Name = "Updated Course", Description = "Updated Description" };

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await courseService.UpdateCourse(courseId, updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while updating course"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
    }

    [Test]
    public async Task UpdateCourse_WhenGeneralExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;
        var updateDto = new UpdateCourseDto { Name = "Updated Course", Description = "Updated Description" };
        var existingCourse = new Course { Id = courseId, Name = "Old Course", Description = "Old Description" };
        var updatedCourse = new Course { Id = courseId, Name = "Updated Course", Description = "Updated Description" };

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync(existingCourse);

        courseRepositoryMock
            .Setup(r => r.UpdateAndCommit(It.IsAny<Course>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await courseService.UpdateCourse(courseId, updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while updating the course"));
        courseRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Course>()), Times.Once);
    }
    #endregion

    #region DeleteCourse Tests
    [Test]
    public async Task DeleteCourse_WithValidId_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Name = "Test Course", Description = "Test Description" };

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync(course);

        courseRepositoryMock
            .Setup(r => r.DeleteAndCommit(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await courseService.DeleteCourse(courseId);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.True);
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
        courseRepositoryMock.Verify(r => r.DeleteAndCommit(courseId), Times.Once);
    }

    [Test]
    public async Task DeleteCourse_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var courseId = 999;

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync((Course)null);

        // Act
        var result = await courseService.DeleteCourse(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Course not found"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
        courseRepositoryMock.Verify(r => r.DeleteAndCommit(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task DeleteCourse_WhenInvalidOperationExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await courseService.DeleteCourse(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while deleting course"));
        courseRepositoryMock.Verify(r => r.Get(courseId), Times.Once);
    }

    [Test]
    public async Task DeleteCourse_WhenGeneralExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Name = "Test Course", Description = "Test Description" };

        courseRepositoryMock
            .Setup(r => r.Get(courseId))
            .ReturnsAsync(course);

        courseRepositoryMock
            .Setup(r => r.DeleteAndCommit(courseId))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await courseService.DeleteCourse(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while deleting the course"));
        courseRepositoryMock.Verify(r => r.DeleteAndCommit(courseId), Times.Once);
    }
    #endregion
}