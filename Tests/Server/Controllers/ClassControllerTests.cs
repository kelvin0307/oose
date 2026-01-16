using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Tests.Server.Controllers;

[TestFixture]
public class ClassControllerTests
{
    private Mock<IClassService> classServiceMock;
    private ClassController controller;

    [SetUp]
    public void Setup()
    {
        classServiceMock = new Mock<IClassService>();
        controller = new ClassController(classServiceMock.Object);
    }

    [Test]
    public async Task GetClass_ReturnsOk_WhenClassExists()
    {
        var classDto = new ClassDTO { Id = 1, Name = "Class A", ClassCode = "A1" };
        classServiceMock.Setup(s => s.GetClassById(1)).ReturnsAsync(Response<ClassDTO>.Ok(classDto));

        var result = await controller.GetClass(1) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Value, Is.EqualTo(classDto));
    }

    [Test]
    public async Task GetStudentsByClass_ReturnsOk_WhenStudentsExist()
    {
        var students = new List<StudentDTO>
        {
            new StudentDTO { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", StudentNumber = 123 }
        };
        classServiceMock.Setup(s => s.GetStudentsByClassId(1)).ReturnsAsync(Response<List<StudentDTO>>.Ok(students));

        var result = await controller.GetStudentsByClass(1) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Value, Is.EqualTo(students));
    }

    //[Test]
    //public async Task GetStudent_ReturnsOk_WhenStudentExistsInClass()
    //{
    //    var student = new StudentDTO { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", StudentNumber = 123 };
    //    classServiceMock.Setup(s => s.GetStudentById(1, 1)).ReturnsAsync(Response<StudentDTO>.Ok(student));

    //    var result = await controller.GetStudent(1, 1) as OkObjectResult;

    //    Assert.That(result, Is.Not.Null);
    //    Assert.That(result.StatusCode, Is.EqualTo(200));
    //    Assert.That(result.Value, Is.EqualTo(student));
    //}
}
