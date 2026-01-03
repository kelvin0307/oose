using NUnit.Framework;
using Core.Services;
using Domain.Models;
using Domain.Enums;
using Core.Tests.TestSupport.Helpers;
using Core.Tests.TestSupport.Helpers.Fakes;

namespace Core.Tests.Services;

[TestFixture]
public class ValidatorServiceTests
{
    private ValidatorService validatorService;
    private List<LearningOutcome> learningOutcomes;

    private const int COURSE_ID = 1;

    [SetUp]
    public void Setup()
    {
        var fakeRepo = new InMemoryLearningOutcomeRepository(() => learningOutcomes);
        validatorService = new ValidatorService(fakeRepo);
    }

    [Test]
    public void ValidateCoursePlanning_NoLearningOutcomes_ReturnsValidationFail()
    {
        learningOutcomes = ValidatorTestHelpers.SetupLearningOutcomes();
        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("Course"), Is.True);
    }

    [Test]
    public void ValidateCoursePlanning_LearningOutcomeWithoutLessons_ReturnsValidationFail()
    {
        learningOutcomes = ValidatorTestHelpers.SetupLearningOutcomes(new LearningOutcome
        {
            Id = 1,
            CourseId = COURSE_ID,
            Name = "LO1",
            Lessons = new List<Lesson>()
        });

        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.True);
    }

    [Test]
    public void ValidateCoursePlanning_LastLessonIsNotTest_ReturnsValidationFail()
    {
        learningOutcomes = ValidatorTestHelpers.SetupLearningOutcomes(new LearningOutcome
        {
            Id = 1,
            CourseId = COURSE_ID,
            Name = "LO1",
            Lessons = new List<Lesson>
            {
                ValidatorTestHelpers.CreateLesson(1, 1, TestType.Written),
                ValidatorTestHelpers.CreateLesson(2, 1) // geen test
            }
        });

        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.True);
    }

    [Test]
    public void ValidateCoursePlanning_ValidCourse_ReturnsOk()
    {
        learningOutcomes = ValidatorTestHelpers.SetupLearningOutcomes(new LearningOutcome
        {
            Id = 1,
            CourseId = COURSE_ID,
            Name = "LO1",
            Lessons = new List<Lesson>
            {
                ValidatorTestHelpers.CreateLesson(1, 1),
                ValidatorTestHelpers.CreateLesson(2, 1, TestType.Practical)
            }
        });

        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.EqualTo("Course planning is valid."));
    }

    [Test]
    public void ValidateCoursePlanning_MultipleLearningOutcomes_OnlyInvalidReturned()
    {
        var validLo = new LearningOutcome
        {
            Id = 1,
            CourseId = COURSE_ID,
            Name = "Valid LO",
            Lessons = new List<Lesson>
            {
                ValidatorTestHelpers.CreateLesson(1, 1),
                ValidatorTestHelpers.CreateLesson(2, 1, TestType.Practical)
            }
        };

        var invalidLo = new LearningOutcome
        {
            Id = 2,
            CourseId = COURSE_ID,
            Name = "Invalid LO",
            Lessons = new List<Lesson>
            {
                ValidatorTestHelpers.CreateLesson(1, 1) // laatste is geen test
            }
        };

        learningOutcomes = ValidatorTestHelpers.SetupLearningOutcomes(validLo, invalidLo);
        var result = validatorService.ValidateCoursePlanning(COURSE_ID);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_2"), Is.True);
        Assert.That(result.ValidationErrors.ContainsKey("LearningOutcome_1"), Is.False);
    }
}
