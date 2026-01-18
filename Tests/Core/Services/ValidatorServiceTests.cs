using Core.Services;
using Core.Common;
using Core.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace Tests.Core.Services;

[TestFixture]
public class ValidatorServiceTests
{
    private Mock<IRepository<LearningOutcome>> learningOutcomeRepositoryMock;
    private Mock<IRubricRepository> rubricRepositoryMock;
    private Mock<IRepository<Planning>> planningRepositoryMock;
    private Mock<IRepository<Course>> courseRepositoryMock;

    private ValidatorService validatorService;

    private const int COURSE_ID = 1;

    [SetUp]
    public void Setup()
    {
        learningOutcomeRepositoryMock = new Mock<IRepository<LearningOutcome>>();
        rubricRepositoryMock = new Mock<IRubricRepository>();
        planningRepositoryMock = new Mock<IRepository<Planning>>();
        courseRepositoryMock = new Mock<IRepository<Course>>();

        validatorService = new ValidatorService(
            learningOutcomeRepositoryMock.Object,
            rubricRepositoryMock.Object,
            planningRepositoryMock.Object,
            courseRepositoryMock.Object
        );
    }

    [Test]
    public async Task ValidateCoursePlanning_NoLearningOutcomes_ReturnsValidationFail()
    {
        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, ICollection<Lesson>>>>()))
            .Returns(new List<LearningOutcome>().AsQueryable());

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<Expression<Func<Planning, ICollection<Lesson>>>>()))
            .Returns(new List<Planning>().AsQueryable());

        var result = await validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("Course"), Is.True);
    }

    [Test]
    public async Task ValidateCoursePlanning_LearningOutcomeWithoutLessons_ReturnsValidationFail()
    {
        var lo = new LearningOutcome
        {
            Id = 1,
            CourseId = COURSE_ID,
            Name = "LO1",
            Lessons = new List<Lesson>()
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, ICollection<Lesson>>>>()))
            .Returns(new List<LearningOutcome> { lo }.AsQueryable());

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<Expression<Func<Planning, ICollection<Lesson>>>>()))
            .Returns(new List<Planning>().AsQueryable());

        rubricRepositoryMock
            .Setup(r => r.GetAggregatesByLearningOutcomeId(lo.Id))
            .ReturnsAsync(new List<Rubric>());

        var result = await validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.True);
    }

    [Test]
    public async Task ValidateCoursePlanning_LastLessonIsNotTest_ReturnsValidationFail()
    {
        var lo = new LearningOutcome
        {
            Id = 1,
            CourseId = COURSE_ID,
            Name = "LO1",
            Lessons = new List<Lesson>
            {
                new Lesson { WeekNumber = 1, SequenceNumber = 1, TestType = TestType.Written },
                new Lesson { WeekNumber = 2, SequenceNumber = 1 }
            }
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, ICollection<Lesson>>>>()))
            .Returns(new List<LearningOutcome> { lo }.AsQueryable());

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<Expression<Func<Planning, ICollection<Lesson>>>>()))
            .Returns(new List<Planning>().AsQueryable());

        rubricRepositoryMock
            .Setup(r => r.GetAggregatesByLearningOutcomeId(lo.Id))
            .ReturnsAsync(new List<Rubric>());

        var result = await validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.True);
    }


}
