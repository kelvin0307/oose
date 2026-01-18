using Core.Common;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;
using System.Collections.Generic;

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
    public async Task AddLearningOutcomesToLesson_ReturnsNoContentOnSuccess()
    {
        lessonServiceMock.Setup(s => s.AddLearningOutcomesToLesson(1, It.IsAny<IList<int>>())).ReturnsAsync(Response<bool>.Ok(true));

        var result = await lessonController.AddLearningOutcomesToLesson(1, new List<int> { 2 });

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task AddLearningOutcomesToLesson_ReturnsErrorWhenFail()
    {
        lessonServiceMock.Setup(s => s.AddLearningOutcomesToLesson(1, It.IsAny<IList<int>>())).ReturnsAsync(Response<bool>.NotFound("Lesson not found"));

        var result = await lessonController.AddLearningOutcomesToLesson(1, new List<int> { 2 });

        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task RemoveLearningOutcomesFromLesson_ReturnsNoContentOnSuccess()
    {
        lessonServiceMock.Setup(s => s.RemoveLearningOutcomesFromLesson(1, It.IsAny<IList<int>>())).ReturnsAsync(Response<bool>.Ok(true));

        var result = await lessonController.RemoveLearningOutcomesFromLesson(1, new List<int> { 2 });

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task RemoveLearningOutcomesFromLesson_ReturnsErrorWhenFail()
    {
        lessonServiceMock.Setup(s => s.RemoveLearningOutcomesFromLesson(1, It.IsAny<IList<int>>())).ReturnsAsync(Response<bool>.Fail("Learning outcome not attached to lesson"));

        var result = await lessonController.RemoveLearningOutcomesFromLesson(1, new List<int> { 2 });

        Assert.That(result, Is.TypeOf<ObjectResult>());
    }
}
