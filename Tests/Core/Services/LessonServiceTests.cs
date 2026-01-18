using System.Linq.Expressions;
using AutoMapper;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;

namespace Tests.Core.Services;

[TestFixture]
public class LessonServiceTests
{
    private Mock<IRepository<Lesson>> lessonRepositoryMock;
    private Mock<IRepository<Planning>> planningRepositoryMock;
    private Mock<IRepository<LearningOutcome>> learningOutcomeRepositoryMock;
    private LessonService lessonService;

    [SetUp]
    public void Setup()
    {
        lessonRepositoryMock = new Mock<IRepository<Lesson>>();
        planningRepositoryMock = new Mock<IRepository<Planning>>();
        learningOutcomeRepositoryMock = new Mock<IRepository<LearningOutcome>>();
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<LessonDTO>(It.IsAny<Lesson>())).Returns((Lesson l) => new LessonDTO { Id = l.Id, Name = l.Name });

        lessonService = new LessonService(lessonRepositoryMock.Object, planningRepositoryMock.Object, learningOutcomeRepositoryMock.Object, mapperMock.Object);
    }


    [Test]
    public async Task AddLearningOutcomesToLesson_WhenLessonNotFound_ReturnsFail()
    {
        lessonRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Lesson, object>>>())).Returns(Enumerable.Empty<Lesson>().AsQueryable());
        lessonRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Lesson>>())).ReturnsAsync((Lesson)null);

        var result = await lessonService.AddLearningOutcomesToLesson(1, new List<int> { 2 });

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Lesson not found"));
    }

    [Test]
    public async Task AddLearningOutcomesToLesson_WhenLearningOutcomeNotFound_ReturnsFail()
    {
        var lesson = new Lesson { Id = 1, Name = "Lesson 1", LearningOutcomes = new List<LearningOutcome>() };
        lessonRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Lesson, object>>>())).Returns(new List<Lesson> { lesson }.AsQueryable());
        lessonRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Lesson>>())).ReturnsAsync(lesson);

        learningOutcomeRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, object>>>())).Returns(Enumerable.Empty<LearningOutcome>().AsQueryable());
        learningOutcomeRepositoryMock.Setup(r => r.ToListAsync(It.IsAny<IQueryable<LearningOutcome>>())).ReturnsAsync(new List<LearningOutcome>());

        var result = await lessonService.AddLearningOutcomesToLesson(1, new List<int> { 2 });

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Learning outcome not found"));
    }

    [Test]
    public async Task AddLearningOutcomesToLesson_WhenAlreadyAttached_SkipsDuplicateAndReturnsNotFound()
    {
        var lo = new LearningOutcome { Id = 2, Name = "LO 1", Lessons = new List<Lesson>() };
        var lesson = new Lesson { Id = 1, Name = "Lesson 1", LearningOutcomes = new List<LearningOutcome> { lo } };
        lo.Lessons.Add(lesson);

        lessonRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Lesson, object>>>())).Returns(new List<Lesson> { lesson }.AsQueryable());
        lessonRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Lesson>>())).ReturnsAsync(lesson);

        learningOutcomeRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, object>>>())).Returns(new List<LearningOutcome> { lo }.AsQueryable());
        learningOutcomeRepositoryMock.Setup(r => r.ToListAsync(It.IsAny<IQueryable<LearningOutcome>>())).ReturnsAsync(new List<LearningOutcome> { lo });

        lessonRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<Lesson>())).ReturnsAsync(lesson);
        learningOutcomeRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>())).ReturnsAsync(lo);

        var result = await lessonService.AddLearningOutcomesToLesson(1, new List<int> { 2 });

        Assert.That(result.Success, Is.False);
        Assert.That(lesson.LearningOutcomes, Has.Count.EqualTo(1));
        lessonRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Lesson>()), Times.Never);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Never);
    }

    [Test]
    public async Task AddLearningOutcomesToLesson_WithNewLearningOutcome_AddsToBotsides()
    {
        var lo = new LearningOutcome { Id = 2, Name = "LO 1", Lessons = new List<Lesson>() };
        var lesson = new Lesson { Id = 1, Name = "Lesson 1", LearningOutcomes = new List<LearningOutcome>() };

        lessonRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Lesson, object>>>())).Returns(new List<Lesson> { lesson }.AsQueryable());
        lessonRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Lesson>>())).ReturnsAsync(lesson);

        learningOutcomeRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, object>>>())).Returns(new List<LearningOutcome> { lo }.AsQueryable());
        learningOutcomeRepositoryMock.Setup(r => r.ToListAsync(It.IsAny<IQueryable<LearningOutcome>>())).ReturnsAsync(new List<LearningOutcome> { lo });

        lessonRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<Lesson>())).ReturnsAsync(lesson);
        learningOutcomeRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>())).ReturnsAsync(lo);

        var result = await lessonService.AddLearningOutcomesToLesson(1, new List<int> { 2 });

        Assert.That(result.Success, Is.True);
        Assert.That(lesson.LearningOutcomes, Has.Count.EqualTo(1));
        Assert.That(lo.Lessons, Has.Count.EqualTo(1));
        lessonRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Lesson>()), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    [Test]
    public async Task RemoveLearningOutcomesFromLesson_WithValidIds_RemovesAndReturnsOk()
    {
        var lo = new LearningOutcome { Id = 2, Name = "LO 1", Lessons = new List<Lesson>() };
        var lesson = new Lesson { Id = 1, Name = "Lesson 1", LearningOutcomes = new List<LearningOutcome> { lo } };
        lo.Lessons.Add(lesson);

        lessonRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Lesson, object>>>())).Returns(new List<Lesson> { lesson }.AsQueryable());
        lessonRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Lesson>>())).ReturnsAsync(lesson);

        learningOutcomeRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, object>>>())).Returns(new List<LearningOutcome> { lo }.AsQueryable());
        learningOutcomeRepositoryMock.Setup(r => r.ToListAsync(It.IsAny<IQueryable<LearningOutcome>>())).ReturnsAsync(new List<LearningOutcome> { lo });

        lessonRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<Lesson>())).ReturnsAsync(lesson);
        learningOutcomeRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>())).ReturnsAsync(lo);

        var result = await lessonService.RemoveLearningOutcomesFromLesson(1, new List<int> { 2 });

        Assert.That(result.Success, Is.True);
        Assert.That(lesson.LearningOutcomes, Has.Count.EqualTo(0));
        Assert.That(lo.Lessons, Has.Count.EqualTo(0));
        lessonRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Lesson>()), Times.Once);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Once);
    }

    [Test]
    public async Task RemoveLearningOutcomesFromLesson_WhenNotAttached_SkipsAndReturnsNotFound()
    {
        var lo = new LearningOutcome { Id = 2, Name = "LO 1", Lessons = new List<Lesson>() };
        var lesson = new Lesson { Id = 1, Name = "Lesson 1", LearningOutcomes = new List<LearningOutcome>() };

        lessonRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<Lesson, object>>>())).Returns(new List<Lesson> { lesson }.AsQueryable());
        lessonRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<IQueryable<Lesson>>())).ReturnsAsync(lesson);

        learningOutcomeRepositoryMock.Setup(r => r.Include(It.IsAny<Expression<Func<LearningOutcome, object>>>())).Returns(new List<LearningOutcome> { lo }.AsQueryable());
        learningOutcomeRepositoryMock.Setup(r => r.ToListAsync(It.IsAny<IQueryable<LearningOutcome>>())).ReturnsAsync(new List<LearningOutcome> { lo });

        lessonRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<Lesson>())).ReturnsAsync(lesson);
        learningOutcomeRepositoryMock.Setup(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>())).ReturnsAsync(lo);

        var result = await lessonService.RemoveLearningOutcomesFromLesson(1, new List<int> { 2 });

        Assert.That(result.Success, Is.False);
        Assert.That(lesson.LearningOutcomes, Has.Count.EqualTo(0));
        Assert.That(lo.Lessons, Has.Count.EqualTo(0));
        lessonRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<Lesson>()), Times.Never);
        learningOutcomeRepositoryMock.Verify(r => r.UpdateAndCommit(It.IsAny<LearningOutcome>()), Times.Never);
    }
}
