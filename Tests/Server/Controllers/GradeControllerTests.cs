using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Tests.Server.Controllers;

[TestFixture]
public class GradeControllerTests
{
    private Mock<IGradeService> gradeServiceMock;
    private GradeController controller;

    [SetUp]
    public void Setup()
    {
        gradeServiceMock = new Mock<IGradeService>();
        controller = new GradeController(gradeServiceMock.Object);
    }

    [Test]
    public async Task CreateGrade_WithValidData_ReturnsCreated()
    {
        var createDto = new CreateGradeDto { Grade = "8", StudentId = 1, LessonId = 1, CourseExecutionId = 1 };
        var gradeDto = new GradeDto { Id = 1, GradeValue = 8, StudentId = 1, LessonId = 1, CourseExecutionId = 1 };

        gradeServiceMock.Setup(s => s.CreateGrade(createDto)).ReturnsAsync(Response<GradeDto>.Ok(gradeDto));

        var result = await controller.CreateGrade(createDto);

        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task UpdateGrade_WithValid_ReturnsOk()
    {
        var updateDto = new UpdateGradeDto { Grade = "9", Feedback = "ok" };
        var responseDto = Response<GradeDto>.Ok(new GradeDto { Id = 1, GradeValue = 9, Feedback = "ok" });

        gradeServiceMock.Setup(s => s.UpdateGrade(It.IsAny<UpdateGradeDto>())).ReturnsAsync(responseDto);

        var result = await controller.UpdateGrade(1, updateDto);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetLatestGrades_ReturnsOk()
    {
        var response = Response<List<GradeDto>>.Ok(new List<GradeDto>());
        gradeServiceMock.Setup(s => s.GetLatestGradesByClassAndExecution(1, 1)).ReturnsAsync(response);

        var result = await controller.GetLatestGradesByClassAndExecution(1, 1);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }
}
