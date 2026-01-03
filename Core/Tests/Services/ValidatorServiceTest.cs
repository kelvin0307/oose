using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Services;
using Core.Interfaces.Repositories;
using Domain.Models;
using Domain.Enums;

namespace Core.Tests.Services
{
    [TestFixture]
    public class ValidatorServiceTests
    {
        private Mock<IRepository<LearningOutcome>> _learningOutcomeRepoMock;
        private Mock<IRepository<Lesson>> _lessonRepoMock;
        private ValidatorService _validatorService;

        [SetUp]
        public void Setup()
        {
            _learningOutcomeRepoMock = new Mock<IRepository<LearningOutcome>>();
            _lessonRepoMock = new Mock<IRepository<Lesson>>();

            _validatorService = new ValidatorService(
                _learningOutcomeRepoMock.Object,
                _lessonRepoMock.Object
            );
        }

        private void SetupLearningOutcomeRepo(IQueryable<LearningOutcome> data)
        {
            _learningOutcomeRepoMock.As<IQueryable<LearningOutcome>>().Setup(m => m.Provider).Returns(data.Provider);
            _learningOutcomeRepoMock.As<IQueryable<LearningOutcome>>().Setup(m => m.Expression).Returns(data.Expression);
            _learningOutcomeRepoMock.As<IQueryable<LearningOutcome>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _learningOutcomeRepoMock.As<IQueryable<LearningOutcome>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupLessonRepo(IQueryable<Lesson> data)
        {
            _lessonRepoMock.As<IQueryable<Lesson>>().Setup(m => m.Provider).Returns(data.Provider);
            _lessonRepoMock.As<IQueryable<Lesson>>().Setup(m => m.Expression).Returns(data.Expression);
            _lessonRepoMock.As<IQueryable<Lesson>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _lessonRepoMock.As<IQueryable<Lesson>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        [Test]
        public async Task ValidateCoursePlanning_NoLearningOutcomes_ReturnsValidationFail()
        {
            int courseId = 1;
            var learningOutcomes = new List<LearningOutcome>().AsQueryable();

            SetupLearningOutcomeRepo(learningOutcomes);

            var result = await _validatorService.ValidateCoursePlanning(courseId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ValidationErrors.ContainsKey("Course"), Is.True);
        }

        [Test]
        public async Task ValidateCoursePlanning_LearningOutcomeWithoutLessons_ReturnsValidationFail()
        {
            int courseId = 1;
            var lo = new LearningOutcome { Id = 1, Name = "LO1", CourseId = courseId };
            var learningOutcomes = new List<LearningOutcome> { lo }.AsQueryable();
            var lessons = new List<Lesson>().AsQueryable(); // geen lessons

            SetupLearningOutcomeRepo(learningOutcomes);
            SetupLessonRepo(lessons);

            var result = await _validatorService.ValidateCoursePlanning(courseId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ValidationErrors.ContainsKey($"LearningOutcome_{lo.Id}"), Is.True);
            Assert.That(result.ValidationErrors[$"LearningOutcome_{lo.Id}"].Any(e => e.Contains("has no lessons")), Is.True);
        }

        [Test]
        public async Task ValidateCoursePlanning_LastLessonNotTest_ReturnsValidationFail()
        {
            int courseId = 1;
            var lo = new LearningOutcome { Id = 1, Name = "LO1", CourseId = courseId };
            var learningOutcomes = new List<LearningOutcome> { lo }.AsQueryable();
            var lessons = new List<Lesson>
            {
                new Lesson { Id = 1, LearningOutcomes = new List<LearningOutcome>{ lo }, WeekNumber = 1, SequenceNumber = 1, TestType = null },
                new Lesson { Id = 2, LearningOutcomes = new List<LearningOutcome>{ lo }, WeekNumber = 2, SequenceNumber = 1, TestType = null } // laatste geen test
            }.AsQueryable();

            SetupLearningOutcomeRepo(learningOutcomes);
            SetupLessonRepo(lessons);

            var result = await _validatorService.ValidateCoursePlanning(courseId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ValidationErrors.ContainsKey($"LearningOutcome_{lo.Id}"), Is.True);
            Assert.That(result.ValidationErrors[$"LearningOutcome_{lo.Id}"].Any(e => e.Contains("is not a test")), Is.True);
        }

        [Test]
        public async Task ValidateCoursePlanning_ValidCourse_ReturnsOk()
        {
            int courseId = 1;
            var lo = new LearningOutcome { Id = 1, Name = "LO1", CourseId = courseId };
            var learningOutcomes = new List<LearningOutcome> { lo }.AsQueryable();
            var lessons = new List<Lesson>
            {
                new Lesson { Id = 1, LearningOutcomes = new List<LearningOutcome>{ lo }, WeekNumber = 1, SequenceNumber = 1, TestType = null },
                new Lesson { Id = 2, LearningOutcomes = new List<LearningOutcome>{ lo }, WeekNumber = 2, SequenceNumber = 1, TestType = TestType.PracticalTest }
            }.AsQueryable();

            SetupLearningOutcomeRepo(learningOutcomes);
            SetupLessonRepo(lessons);

            var result = await _validatorService.ValidateCoursePlanning(courseId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Result, Is.EqualTo("Course planning is valid."));
        }

        [Test]
        public async Task ValidateCoursePlanning_MultipleLearningOutcomes_AllValidatedCorrectly()
        {
            int courseId = 1;
            var lo1 = new LearningOutcome { Id = 1, Name = "LO1", CourseId = courseId };
            var lo2 = new LearningOutcome { Id = 2, Name = "LO2", CourseId = courseId };
            var learningOutcomes = new List<LearningOutcome> { lo1, lo2 }.AsQueryable();

            var lessons = new List<Lesson>
            {
                new Lesson { Id = 1, LearningOutcomes = new List<LearningOutcome>{ lo1 }, WeekNumber = 1, SequenceNumber = 1, TestType = null },
                new Lesson { Id = 2, LearningOutcomes = new List<LearningOutcome>{ lo1 }, WeekNumber = 2, SequenceNumber = 1, TestType = TestType.PracticalTest },
                new Lesson { Id = 3, LearningOutcomes = new List<LearningOutcome>{ lo2 }, WeekNumber = 1, SequenceNumber = 1, TestType = null } // laatste voor lo2 geen test
            }.AsQueryable();

            SetupLearningOutcomeRepo(learningOutcomes);
            SetupLessonRepo(lessons);

            var result = await _validatorService.ValidateCoursePlanning(courseId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ValidationErrors.ContainsKey($"LearningOutcome_{lo2.Id}"), Is.True);
        }
    }
}
