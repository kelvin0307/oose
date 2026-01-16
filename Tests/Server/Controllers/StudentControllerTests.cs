using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;
using System.Threading.Tasks;

namespace Tests.Server.Controllers;

[TestFixture]
public class StudentControllerTests
{
    private Mock<IStudentService> studentServiceMock;
    private StudentController controller;

    [SetUp]
    public void Setup()
    {
        studentServiceMock = new Mock<IStudentService>();
        controller = new StudentController(studentServiceMock.Object);
    }

    [Test]
    public async Task GetStudent_ReturnsOk_WhenStudentExists()
    {
        var studentDto = new StudentDTO { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", StudentNumber = 456 };
        studentServiceMock.Setup(s => s.GetStudentById(1)).ReturnsAsync(Response<StudentDTO>.Ok(studentDto));

        var result = await controller.GetStudent(1) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Value, Is.EqualTo(studentDto));
    }

    [Test]
    public async Task GetStudent_ReturnsNotFound_WhenStudentDoesNotExist()
    {
        studentServiceMock.Setup(s => s.GetStudentById(2)).ReturnsAsync(Response<StudentDTO>.NotFound("Student not found"));

        var actionResult = await controller.GetStudent(2);

        Assert.That(actionResult, Is.Not.Null);
        Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
    }
}
