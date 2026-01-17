using AutoMapper;
using Core.Common;
using Core.DTOs;
using Data.Interfaces.Repositories;
using Core.Services;
using Domain.Enums;
using Domain.Models;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace Tests.Core.Services;

[TestFixture]
public class GradeServiceTests
{
    private Mock<IRepository<Grade>> gradeRepositoryMock;
    private Mock<IRepository<Student>> studentRepositoryMock;
    private Mock<IRepository<Lesson>> lessonRepositoryMock;
    private Mock<IRepository<CourseExecution>> courseExecutionRepositoryMock;
    private Mock<IMapper> mapperMock;
    private GradeService gradeService;

    [SetUp]
    public void Setup()
    {
        gradeRepositoryMock = new Mock<IRepository<Grade>>();
        studentRepositoryMock = new Mock<IRepository<Student>>();
        lessonRepositoryMock = new Mock<IRepository<Lesson>>();
        courseExecutionRepositoryMock = new Mock<IRepository<CourseExecution>>();
        mapperMock = new Mock<IMapper>();
        gradeService = new GradeService(
            gradeRepositoryMock.Object,
            studentRepositoryMock.Object,
            lessonRepositoryMock.Object,
            courseExecutionRepositoryMock.Object,
            mapperMock.Object);
    }

    #region CreateGrade Tests

    [Test]
    public async Task CreateGrade_WithLetterGrade_ConvertsToNumeric()
    {
        // Arrange
        var createDto = new CreateGradeDTO { Grade = "V", StudentId = 1, LessonId = 1, CourseExecutionId = 1 };
        var student = new Student { Id = 1, FirstName = "A", LastName = "Z" };
        var lesson = new Lesson { Id = 1, TestType = TestType.Practical };
        var grade = new Grade { Id = 1, GradeValue = 8, StudentId = 1, LessonId = 1, CourseExcecutionId = 1, Student = student };
        var gradeDto = new GradeDTO { Id = 1, GradeValue = 8, StudentId = 1, LessonId = 1, CourseExecutionId = 1, StudentFirstName = "A", StudentLastName = "Z" };

        lessonRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(lesson);
        studentRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(student);
        gradeRepositoryMock.Setup(r => r.CreateAndCommit(It.IsAny<Grade>())).ReturnsAsync((Grade g) => { g.Id = 1; return g; });
        mapperMock.Setup(m => m.Map<GradeDTO>(It.IsAny<Grade>())).Returns(gradeDto);

        // Act
        var result = await gradeService.CreateGrade(createDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.GradeValue, Is.EqualTo(8));
        lessonRepositoryMock.Verify(r => r.Get(1), Times.Once);
        studentRepositoryMock.Verify(r => r.Get(1), Times.Once);
        gradeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<Grade>()), Times.Once);
        mapperMock.Verify(m => m.Map<GradeDTO>(It.IsAny<Grade>()), Times.Once);
    }

    [Test]
    public async Task CreateGrade_WithoutGrade_ReturnsFail()
    {
        // Arrange
        var createDto = new CreateGradeDTO { Grade = "", StudentId = 1, LessonId = 1, CourseExecutionId = 1 };

        // Act
        var result = await gradeService.CreateGrade(createDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid or missing grade"));
        lessonRepositoryMock.Verify(r => r.Get(It.IsAny<int>()), Times.Never);
        gradeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<Grade>()), Times.Never);
    }

    [Test]
    public async Task CreateGrade_NonExamLesson_ReturnsFail()
    {
        // Arrange
        var createDto = new CreateGradeDTO { Grade = "8", StudentId = 1, LessonId = 1, CourseExecutionId = 1 };
        var lesson = new Lesson { Id = 1, TestType = null };

        lessonRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(lesson);

        // Act
        var result = await gradeService.CreateGrade(createDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Only exam lessons"));
        lessonRepositoryMock.Verify(r => r.Get(1), Times.Once);
        gradeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<Grade>()), Times.Never);
    }

    [Test]
    public async Task CreateGrade_WithNumericGrade_CreatesSuccessfully()
    {
        // Arrange
        var createDto = new CreateGradeDTO { Grade = "8", StudentId = 1, LessonId = 1, CourseExecutionId = 1 };
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe" };
        var lesson = new Lesson { Id = 1, TestType = TestType.Written };
        var grade = new Grade { Id = 1, GradeValue = 8, StudentId = 1, LessonId = 1, CourseExcecutionId = 1, Student = student };
        var gradeDto = new GradeDTO { Id = 1, GradeValue = 8, StudentId = 1, LessonId = 1, CourseExecutionId = 1, StudentFirstName = "John", StudentLastName = "Doe" };

        lessonRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(lesson);
        studentRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(student);
        gradeRepositoryMock.Setup(r => r.CreateAndCommit(It.IsAny<Grade>())).ReturnsAsync((Grade g) => { g.Id = 1; return g; });
        mapperMock.Setup(m => m.Map<GradeDTO>(It.IsAny<Grade>())).Returns(gradeDto);

        // Act
        var result = await gradeService.CreateGrade(createDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.GradeValue, Is.EqualTo(8));
        gradeRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<Grade>()), Times.Once);
    }

    #endregion

    #region UpdateGrade Tests

    [Test]
    public async Task UpdateGrade_OverwritesGradeAndFeedback()
    {
        // Arrange
        var existing = new Grade { Id = 1, GradeValue = 5, Feedback = "old", StudentId = 1, LessonId = 1, CourseExcecutionId = 1 };
        var student = new Student { Id = 1, FirstName = "B", LastName = "A" };
        var updatedGrade = new Grade { Id = 1, GradeValue = 10, Feedback = "new", StudentId = 1, LessonId = 1, CourseExcecutionId = 1, Student = student };
        var gradeDto = new GradeDTO { Id = 1, GradeValue = 10, Feedback = "new", StudentId = 1, LessonId = 1, CourseExecutionId = 1, StudentFirstName = "B", StudentLastName = "A" };
        var updateDto = new UpdateGradeDTO { Id = 1, Grade = "10", Feedback = "new" };

        gradeRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(existing);
        gradeRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<Grade>())).ReturnsAsync(updatedGrade);
        
        // Mock the Include method to return a queryable that contains the updated grade
        var gradeQueryable = new List<Grade> { updatedGrade }.AsQueryable();
        gradeRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Grade, object>>>()))
            .Returns(gradeQueryable);
        
        mapperMock.Setup(m => m.Map<GradeDTO>(It.IsAny<Grade>())).Returns(gradeDto);

        // Act
        var result = await gradeService.UpdateGrade(updateDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.GradeValue, Is.EqualTo(10));
        Assert.That(result.Result.Feedback, Is.EqualTo("new"));
        gradeRepositoryMock.Verify(r => r.Get(1), Times.Once);
        gradeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Grade>()), Times.Once);
    }

    [Test]
    public async Task UpdateGrade_WithNonExistentGrade_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateGradeDTO { Id = 999, Grade = "10", Feedback = "new" };

        gradeRepositoryMock.Setup(r => r.Get(999)).ReturnsAsync((Grade)null);

        // Act
        var result = await gradeService.UpdateGrade(updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Grade not found"));
        gradeRepositoryMock.Verify(r => r.Get(999), Times.Once);
        gradeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Grade>()), Times.Never);
    }

    [Test]
    public async Task UpdateGrade_WithInvalidGrade_ReturnsFail()
    {
        // Arrange
        var existing = new Grade { Id = 1, GradeValue = 5, Feedback = "old", StudentId = 1, LessonId = 1, CourseExcecutionId = 1 };
        var updateDto = new UpdateGradeDTO { Id = 1, Grade = "invalid", Feedback = "new" };

        gradeRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(existing);

        // Act
        var result = await gradeService.UpdateGrade(updateDto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid or missing grade"));
        gradeRepositoryMock.Verify(r => r.Get(1), Times.Once);
        gradeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Grade>()), Times.Never);
    }

    #endregion

    #region GetLatestGradesByClassAndExecution Tests

    [Test]
    public async Task GetLatestGradesByClassAndExecution_ReturnsOnlyLatestPerLesson_SortedByLastName()
    {
        // Arrange
        var classId = 1;
        var courseExecutionId = 1;
        var students = new List<Student>
        {
            new Student { Id = 1, FirstName = "Alice", LastName = "Zephyr", ClassId = classId },
            new Student { Id = 2, FirstName = "Bob", LastName = "Young", ClassId = classId }
        };

        var grades = new List<Grade>
        {
            new Grade { Id = 1, StudentId = 1, LessonId = 1, CourseExcecutionId = courseExecutionId, GradeValue = 5, Student = students[0] },
            new Grade { Id = 2, StudentId = 1, LessonId = 1, CourseExcecutionId = courseExecutionId, GradeValue = 7, Student = students[0] }, // latest for student 1, lesson 1
            new Grade { Id = 3, StudentId = 2, LessonId = 1, CourseExcecutionId = courseExecutionId, GradeValue = 9, Student = students[1] }
        };

        var gradeDtos = new List<GradeDTO>
        {
            new GradeDTO { Id = 2, StudentId = 1, LessonId = 1, CourseExecutionId = courseExecutionId, GradeValue = 7, StudentFirstName = "Alice", StudentLastName = "Zephyr" },
            new GradeDTO { Id = 3, StudentId = 2, LessonId = 1, CourseExecutionId = courseExecutionId, GradeValue = 9, StudentFirstName = "Bob", StudentLastName = "Young" }
        };

        // Setup Include to return a queryable of filtered grades
        var gradesQueryable = grades.AsQueryable();
        gradeRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Grade, Student>>>()))
            .Returns(gradesQueryable);
        
        // Setup ToListAsync to return the filtered grades
        gradeRepositoryMock.Setup(r => r.ToListAsync(It.IsAny<IQueryable<Grade>>()))
            .ReturnsAsync(grades);

        mapperMock.Setup(m => m.Map<GradeDTO>(grades[1])).Returns(gradeDtos[0]);
        mapperMock.Setup(m => m.Map<GradeDTO>(grades[2])).Returns(gradeDtos[1]);

        // Act
        var result = await gradeService.GetLatestGradesByClassAndExecution(classId, courseExecutionId);

        // Assert
        Assert.That(result.Success, Is.True);
        var list = result.Result;
        Assert.That(list.Count, Is.EqualTo(2));
        // Sorted by last name: Young then Zephyr
        Assert.That(list[0].StudentLastName, Is.EqualTo("Young"));
        Assert.That(list[1].StudentLastName, Is.EqualTo("Zephyr"));
        // Latest grade for student 1 should be 7
        Assert.That(list.First(g => g.StudentId == 1).GradeValue, Is.EqualTo(7));
        gradeRepositoryMock.Verify(r => r.Include(It.IsAny<Expression<Func<Grade, Student>>>()), Times.Once);
        gradeRepositoryMock.Verify(r => r.ToListAsync(It.IsAny<IQueryable<Grade>>()), Times.Once);
    }

    #endregion
}
