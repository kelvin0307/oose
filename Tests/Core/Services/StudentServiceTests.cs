using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests.Core.Services;

[TestFixture]
public class StudentServiceTests
{
    private Mock<IRepository<Student>> studentRepositoryMock;
    private Mock<IRepository<Class>> classRepositoryMock;
    private Mock<IMapper> mapperMock;
    private IStudentService studentService;

    [SetUp]
    public void Setup()
    {
        studentRepositoryMock = new Mock<IRepository<Student>>();
        classRepositoryMock = new Mock<IRepository<Class>>();
        mapperMock = new Mock<IMapper>();
        studentService = new StudentService(classRepositoryMock.Object, studentRepositoryMock.Object, mapperMock.Object);
    }

    [Test]
    public async Task GetStudentById_ReturnsOk_WhenStudentExists()
    {
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", StudentNumber = 123 };
        var studentDto = new StudentDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", StudentNumber = 123 };

        studentRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(student);
        mapperMock.Setup(m => m.Map<StudentDto>(student)).Returns(studentDto);

        var result = await studentService.GetStudentById(1);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.EqualTo(studentDto));
    }

    [Test]
    public async Task GetStudentById_ReturnsNotFound_WhenStudentDoesNotExist()
    {
        studentRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync((Student)null);

        var result = await studentService.GetStudentById(1);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Student not found"));
    }

    [Test]
    public async Task GetStudentById_ReturnsFail_OnInvalidOperationException()
    {
        studentRepositoryMock.Setup(r => r.Get(1)).ThrowsAsync(new InvalidOperationException());

        var result = await studentService.GetStudentById(1);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while fetching student"));
    }

    [Test]
    public async Task GetStudentById_ReturnsFail_OnException()
    {
        studentRepositoryMock.Setup(r => r.Get(1)).ThrowsAsync(new Exception("boom"));

        var result = await studentService.GetStudentById(1);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching the student"));
    }
}
