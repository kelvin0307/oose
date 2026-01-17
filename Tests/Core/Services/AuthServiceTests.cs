using Core.DTOs;
using Core.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;
using Data.Interfaces.Repositories;

namespace Tests.Core.Services;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IRepository<Teacher>> teacherRepositoryMock;
    private AuthService authService;

    [SetUp]
    public void Setup()
    {
        teacherRepositoryMock = new Mock<IRepository<Teacher>>();
        authService = new AuthService(teacherRepositoryMock.Object);
    }

    #region LoginTeacherByEmail Tests

    [Test]
    public async Task LoginTeacherByEmail_WithValidEmail_ReturnsSuccessResponse()
    {
        // Arrange
        var email = "teacher@example.com";
        var teacher = new Teacher
        {
            Id = 1,
            FirstName = "John",
            MiddleName = "Michael",
            LastName = "Doe",
            Email = email,
            TeacherCode = "T001"
        };

        teacherRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await authService.LoginTeacherByEmail(email);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Id, Is.EqualTo(teacher.Id));
        Assert.That(result.Result.Email, Is.EqualTo(email));
        Assert.That(result.Result.FirstName, Is.EqualTo("John"));
        Assert.That(result.Result.TeacherCode, Is.EqualTo("T001"));
        teacherRepositoryMock.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()), Times.Once);
    }

    [Test]
    public async Task LoginTeacherByEmail_WithNullEmail_ReturnsFail()
    {
        // Act
        var result = await authService.LoginTeacherByEmail(null!);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Email is required"));
        teacherRepositoryMock.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()), Times.Never);
    }

    [Test]
    public async Task LoginTeacherByEmail_WithEmptyEmail_ReturnsFail()
    {
        // Act
        var result = await authService.LoginTeacherByEmail("");

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Email is required"));
        teacherRepositoryMock.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()), Times.Never);
    }

    [Test]
    public async Task LoginTeacherByEmail_WithWhitespaceEmail_ReturnsFail()
    {
        // Act
        var result = await authService.LoginTeacherByEmail("   ");

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Email is required"));
        teacherRepositoryMock.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()), Times.Never);
    }

    [Test]
    public async Task LoginTeacherByEmail_WithNonExistentEmail_ReturnsNotFound()
    {
        // Arrange
        var email = "nonexistent@example.com";
        teacherRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()))
            .ReturnsAsync((Teacher)null!);

        // Act
        var result = await authService.LoginTeacherByEmail(email);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("No teacher found with email"));
        teacherRepositoryMock.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()), Times.Once);
    }

    [Test]
    public async Task LoginTeacherByEmail_WithEmailHavingWhitespace_TrimsEmail()
    {
        // Arrange
        var emailWithWhitespace = "  teacher@example.com  ";
        var teacher = new Teacher
        {
            Id = 1,
            FirstName = "Jane",
            MiddleName = "Anne",
            LastName = "Smith",
            Email = "teacher@example.com",
            TeacherCode = "T002"
        };

        teacherRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await authService.LoginTeacherByEmail(emailWithWhitespace);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Email, Is.EqualTo("teacher@example.com"));
        teacherRepositoryMock.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()), Times.Once);
    }

    [Test]
    public async Task LoginTeacherByEmail_WhenRepositoryThrowsException_ReturnsFail()
    {
        // Arrange
        var email = "teacher@example.com";
        teacherRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await authService.LoginTeacherByEmail(email);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An error occurred during login"));
        teacherRepositoryMock.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()), Times.Once);
    }

    [Test]
    public async Task LoginTeacherByEmail_WithCompleteTeacherInfo_MapsAllFields()
    {
        // Arrange
        var email = "complete@example.com";
        var teacher = new Teacher
        {
            Id = 42,
            FirstName = "Robert",
            MiddleName = "James",
            LastName = "Johnson",
            Email = email,
            TeacherCode = "T042"
        };

        teacherRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Teacher, bool>>>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await authService.LoginTeacherByEmail(email);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Id, Is.EqualTo(42));
        Assert.That(result.Result.FirstName, Is.EqualTo("Robert"));
        Assert.That(result.Result.MiddleName, Is.EqualTo("James"));
        Assert.That(result.Result.LastName, Is.EqualTo("Johnson"));
        Assert.That(result.Result.Email, Is.EqualTo(email));
        Assert.That(result.Result.TeacherCode, Is.EqualTo("T042"));
    }

    #endregion
}
