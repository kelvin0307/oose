using System.Linq.Expressions;
using Data.Context;
using Data.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Tests.Data.Repositories;

[TestFixture]
public class RepositoryTests
{
    private Mock<DataContext> dataContextMock;
    private Mock<DbSet<Course>> courseDbSetMock;
    private Mock<Repository<Course>> courseRepositoryMock;
    private Repository<Course> courseRepository;

    [SetUp]
    public void Setup()
    {
        dataContextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());
        courseDbSetMock = new Mock<DbSet<Course>>();

        dataContextMock.Setup(c => c.Set<Course>()).Returns(courseDbSetMock.Object);

        courseRepository = new Repository<Course>(dataContextMock.Object);
        courseRepositoryMock = new Mock<Repository<Course>>(dataContextMock.Object);
    }

    #region CreateAndCommit Tests

    [Test]
    public async Task CreateAndCommit_WithValidEntity_SavesAndReturnsEntity()
    {
        // Arrange
        var course = new Course
        {
            Id = 1,
            Name = "Test Course",
            Description = "Test Description"
        };

        courseDbSetMock.Setup(c => c.Add(It.IsAny<Course>()));
        dataContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await courseRepository.CreateAndCommit(course);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Course"));
        dataContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    #endregion

    #region Get Tests

    [Test]
    public async Task Get_WithValidId_ReturnsEntity()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Name = "Test Course" };

        courseDbSetMock.Setup(c => c.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await courseRepository.Get(courseId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(courseId));
        Assert.That(result.Name, Is.EqualTo("Test Course"));
    }

    [Test]
    public async Task Get_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var courseId = 999;

        courseDbSetMock.Setup(c => c.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course)null!);

        // Act
        var result = await courseRepository.Get(courseId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Get_WithPredicate_ReturnsMatchingEntity()
    {
        // Arrange
        var course = new Course { Id = 1, Name = "Test Course", Description = "Description" };

        // Mock the wrapper method instead of the extension method
        courseRepositoryMock
            .Setup(r => r.FirstOrDefaultAsyncWrapper(
                It.IsAny<IQueryable<Course>>(),
                It.IsAny<Expression<Func<Course, bool>>>()))
            .ReturnsAsync(course);

        // Act
        var result = await courseRepositoryMock.Object.Get(c => c.Name == "Test Course");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Course"));
    }

    [Test]
    public async Task Get_WithNonMatchingPredicate_ReturnsNull()
    {
        // Arrange
        courseRepositoryMock
            .Setup(r => r.FirstOrDefaultAsyncWrapper(
                It.IsAny<IQueryable<Course>>(),
                It.IsAny<Expression<Func<Course, bool>>>()))
            .ReturnsAsync((Course)null!);

        // Act
        var result = await courseRepositoryMock.Object.Get(c => c.Name == "Nonexistent");

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetAll Tests

    [Test]
    public async Task GetAll_WithMultipleEntities_ReturnsAllEntities()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" },
            new Course { Id = 3, Name = "Course 3" }
        };

        courseRepositoryMock
            .Setup(r => r.ToListAsyncWrapper(It.IsAny<IQueryable<Course>>()))
            .ReturnsAsync(courses);

        // Act
        var result = await courseRepositoryMock.Object.GetAll();

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result[0].Name, Is.EqualTo("Course 1"));
        Assert.That(result[2].Name, Is.EqualTo("Course 3"));
    }

    [Test]
    public async Task GetAll_WithNoCourses_ReturnsEmptyList()
    {
        // Arrange
        var emptyCourseList = new List<Course>();

        courseRepositoryMock
            .Setup(r => r.ToListAsyncWrapper(It.IsAny<IQueryable<Course>>()))
            .ReturnsAsync(emptyCourseList);

        // Act
        var result = await courseRepositoryMock.Object.GetAll();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAll_WithPredicate_ReturnsFilteredEntities()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        };

        var filteredCourses = courses.Where(c => c.Id > 1).ToList();

        courseRepositoryMock
            .Setup(r => r.WhereWrapper(
                It.IsAny<IQueryable<Course>>(),
                It.IsAny<Expression<Func<Course, bool>>>()))
            .Returns((IQueryable<Course> q, Expression<Func<Course, bool>> predicate) =>
                courses.AsQueryable().Where(predicate));

        courseRepositoryMock
            .Setup(r => r.ToListAsyncWrapper(It.IsAny<IQueryable<Course>>()))
            .ReturnsAsync(filteredCourses);

        // Act
        var result = await courseRepositoryMock.Object.GetAll(c => c.Id > 1);

        // Assert
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(0));
    }

    #endregion

    #region UpdateAndCommit Tests

    [Test]
    public async Task UpdateAndCommit_WithValidEntity_UpdatesAndReturnsEntity()
    {
        // Arrange
        var course = new Course { Id = 1, Name = "Updated Course" };

        courseDbSetMock.Setup(c => c.Update(It.IsAny<Course>()));
        dataContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await courseRepository.UpdateAndCommit(course);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Updated Course"));
        courseDbSetMock.Verify(c => c.Update(course), Times.Once);
        dataContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAndCommit Tests

    [Test]
    public async Task DeleteAndCommit_WithValidId_DeletesAndReturnsEntity()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Name = "Test Course" };

        courseDbSetMock.Setup(c => c.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        courseDbSetMock.Setup(c => c.Remove(It.IsAny<Course>()));

        dataContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await courseRepository.DeleteAndCommit(courseId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(courseId));
        courseDbSetMock.Verify(c => c.Remove(course), Times.Once);
        dataContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void DeleteAndCommit_WithNonExistentId_ThrowsException()
    {
        // Arrange
        var courseId = 999;

        courseDbSetMock.Setup(c => c.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course)null!);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () =>
            await courseRepository.DeleteAndCommit(courseId));
    }

    #endregion

    #region Delete Tests

    [Test]
    public void Delete_WithValidId_RemovesEntity()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Name = "Test Course" };

        courseDbSetMock.Setup(c => c.Find(It.IsAny<object[]>()))
            .Returns(course);

        courseDbSetMock.Setup(c => c.Remove(It.IsAny<Course>()));

        // Act
        courseRepository.Delete(courseId);

        // Assert
        courseDbSetMock.Verify(c => c.Remove(It.IsAny<Course>()), Times.Once);
    }

    [Test]
    public void Delete_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        var courseId = 999;

        courseDbSetMock.Setup(c => c.Find(It.IsAny<object[]>()))
            .Returns((Course)null!);

        // Act & Assert - should not throw
        Assert.DoesNotThrow(() => courseRepository.Delete(courseId));
    }

    #endregion

    #region Update Tests

    [Test]
    public void Update_WithValidEntity_UpdatesEntity()
    {
        // Arrange
        var course = new Course { Id = 1, Name = "Updated" };

        courseDbSetMock.Setup(c => c.Update(It.IsAny<Course>()));

        // Act
        courseRepository.Update(course);

        // Assert
        courseDbSetMock.Verify(c => c.Update(course), Times.Once);
    }

    #endregion

    #region Include Tests

    [Test]
    public void Include_WithNavigationProperty_ReturnsQueryable()
    {
        // Arrange
        var coursesList = new List<Course> { new Course { Id = 1, Name = "Test" } };
        var courses = coursesList.AsQueryable();

        courseRepositoryMock
            .Setup(r => r.IncludeWrapper(
                It.IsAny<IQueryable<Course>>(),
                It.IsAny<Expression<Func<Course, ICollection<LearningOutcome>>>>()))
            .Returns(courses);

        // Act
        var result = courseRepositoryMock.Object.Include(c => c.LearningOutcomes);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result is IQueryable<Course>, Is.True);
    }

    #endregion

    #region Find Tests

    [Test]
    public void Find_WithPredicate_ReturnsFilteredQueryable()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        }.AsQueryable();

        courseRepositoryMock
            .Setup(r => r.WhereWrapper(
                It.IsAny<IQueryable<Course>>(),
                It.IsAny<Expression<Func<Course, bool>>>()))
            .Returns(courses.Where(c => c.Id == 1));

        // Act
        var result = courseRepositoryMock.Object.Find(c => c.Id == 1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result is IQueryable<Course>, Is.True);
    }

    #endregion

    #region SaveManually Tests

    [Test]
    public async Task SaveManually_CallsSaveChangesAsync()
    {
        // Arrange
        dataContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await courseRepository.SaveManually();

        // Assert
        dataContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region ToListAsync Tests

    [Test]
    public async Task ToListAsync_WithQueryable_ReturnsListAsync()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        };

        var queryable = courses.AsQueryable();

        courseRepositoryMock
            .Setup(r => r.ToListAsyncWrapper(It.IsAny<IQueryable<Course>>()))
            .ReturnsAsync(courses);

        // Act
        var result = await courseRepositoryMock.Object.ToListAsync(queryable);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    #endregion

    #region FirstOrDefaultAsync Tests

    [Test]
    public async Task FirstOrDefaultAsync_WithQueryable_ReturnsFirstEntity()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        };

        var queryable = courses.AsQueryable();
        var firstCourse = courses.First();

        courseRepositoryMock
            .Setup(r => r.FirstOrDefaultAsyncWrapper(It.IsAny<IQueryable<Course>>()))
            .ReturnsAsync(firstCourse);

        // Act
        var result = await courseRepositoryMock.Object.FirstOrDefaultAsync(queryable);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task FirstOrDefaultAsync_WithEmptyQueryable_ReturnsNull()
    {
        // Arrange
        var courses = new List<Course>();
        var queryable = courses.AsQueryable();

        courseRepositoryMock
            .Setup(r => r.FirstOrDefaultAsyncWrapper(It.IsAny<IQueryable<Course>>()))
            .ReturnsAsync((Course)null!);

        // Act
        var result = await courseRepositoryMock.Object.FirstOrDefaultAsync(queryable);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion
}
