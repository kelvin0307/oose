using Core.Common;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Tests.Server.Controllers;

[TestFixture]
public class LessonControllerTests
{
    private Mock<ILessonService> lessonServiceMock;
    private LessonController lessonController;

    [SetUp]
    public void Setup()
    {
        lessonServiceMock = new Mock<ILessonService>();
        lessonController = new LessonController(lessonServiceMock.Object);
    }

    [Test]
    public async Task AddLearningOutcomeToLesson_ReturnsNoContentOnSuccess()
    {
        lessonServiceMock.Setup(s => s.AddLearningOutcomeToLesson(1, 2)).ReturnsAsync(Response<bool>.Ok(true));

        var result = await lessonController.AddLearningOutcomeToLesson(1, 2);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task AddLearningOutcomeToLesson_ReturnsErrorWhenFail()
    {
        lessonServiceMock.Setup(s => s.AddLearningOutcomeToLesson(1, 2)).ReturnsAsync(Response<bool>.NotFound("Lesson not found"));

        var result = await lessonController.AddLearningOutcomeToLesson(1, 2);

        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task RemoveLearningOutcomeFromLesson_ReturnsNoContentOnSuccess()
    {
        lessonServiceMock.Setup(s => s.RemoveLearningOutcomeFromLesson(1, 2)).ReturnsAsync(Response<bool>.Ok(true));

        var result = await lessonController.RemoveLearningOutcomeFromLesson(1, 2);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task RemoveLearningOutcomeFromLesson_ReturnsErrorWhenFail()
    {
        lessonServiceMock.Setup(s => s.RemoveLearningOutcomeFromLesson(1, 2)).ReturnsAsync(Response<bool>.Fail("Learning outcome not attached to lesson"));

        var result = await lessonController.RemoveLearningOutcomeFromLesson(1, 2);

        Assert.That(result, Is.TypeOf<ObjectResult>());
    }
}
