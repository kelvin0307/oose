using Core.Services;
using Domain.Enums;
using Domain.Models;
using Moq;
using NUnit.Framework;
using Data.Interfaces.Repositories;

namespace Tests.Core.Services;

[TestFixture]
public class ValidatorServiceTests
{
    private Mock<IRepository<LearningOutcome>> learningOutcomeRepositoryMock;
    private ValidatorService validatorService;

    private const int COURSE_ID = 1;

    [SetUp]
    public void Setup()
    {
        learningOutcomeRepositoryMock = new Mock<IRepository<LearningOutcome>>();
        validatorService = new ValidatorService(learningOutcomeRepositoryMock.Object);
    }

    [Test]
    public void ValidateCoursePlanning_NoLearningOutcomes_ReturnsValidationFail()
    {
        // Arrange
        var learningOutcomes = new List<LearningOutcome>().AsQueryable();

        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<LearningOutcome, System.Collections.Generic.ICollection<Lesson>>>>()))
            .Returns(learningOutcomes);

        // Act
        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("Course"), Is.True);
    }

    [Test]
    public void ValidateCoursePlanning_LearningOutcomeWithoutLessons_ReturnsValidationFail()
    {
        // Arrange
        var learningOutcomes = new List<LearningOutcome>
        {
            new LearningOutcome
            {
                Id = 1,
                CourseId = COURSE_ID,
                Name = "LO1",
                Lessons = new List<Lesson>()
            }
        }.AsQueryable();

        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<LearningOutcome, System.Collections.Generic.ICollection<Lesson>>>>()))
            .Returns(learningOutcomes);

        // Act
        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.True);
    }

    [Test]
    public void ValidateCoursePlanning_LastLessonIsNotTest_ReturnsValidationFail()
    {
        // Arrange
        var learningOutcomes = new List<LearningOutcome>
        {
            new LearningOutcome
            {
                Id = 1,
                CourseId = COURSE_ID,
                Name = "LO1",
                Lessons = new List<Lesson>
                {
                    new Lesson { WeekNumber = 1, SequenceNumber = 1, TestType = TestType.Written },
                    new Lesson { WeekNumber = 2, SequenceNumber = 1 }
                }
            }
        }.AsQueryable();

        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<LearningOutcome, System.Collections.Generic.ICollection<Lesson>>>>()))
            .Returns(learningOutcomes);

        // Act
        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.True);
    }

    [Test]
    public void ValidateCoursePlanning_ValidCourse_ReturnsOk()
    {
        // Arrange
        var learningOutcomes = new List<LearningOutcome>
        {
            new LearningOutcome
            {
                Id = 1,
                CourseId = COURSE_ID,
                Name = "LO1",
                Lessons = new List<Lesson>
                {
                    new Lesson { WeekNumber = 1, SequenceNumber = 1 },
                    new Lesson { WeekNumber = 2, SequenceNumber = 1, TestType = TestType.Practical }
                }
            }
        }.AsQueryable();

        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<LearningOutcome, System.Collections.Generic.ICollection<Lesson>>>>()))
            .Returns(learningOutcomes);

        // Act
        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.EqualTo("Course planning is valid."));
    }

    [Test]
    public void ValidateCoursePlanning_MultipleLearningOutcomes_OnlyInvalidReturned()
    {
        // Arrange
        var learningOutcomes = new List<LearningOutcome>
        {
            new LearningOutcome
            {
                Id = 1,
                CourseId = COURSE_ID,
                Name = "Valid LO",
                Lessons = new List<Lesson>
                {
                    new Lesson { WeekNumber = 1, SequenceNumber = 1 },
                    new Lesson { WeekNumber = 2, SequenceNumber = 1, TestType = TestType.Practical }
                }
            },
            new LearningOutcome
            {
                Id = 2,
                CourseId = COURSE_ID,
                Name = "Invalid LO",
                Lessons = new List<Lesson>
                {
                    new Lesson { WeekNumber = 1, SequenceNumber = 1 }
                }
            }
        }.AsQueryable();

        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<LearningOutcome, System.Collections.Generic.ICollection<Lesson>>>>()))
            .Returns(learningOutcomes);

        // Act
        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_2"), Is.True);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.False);
    }
}
