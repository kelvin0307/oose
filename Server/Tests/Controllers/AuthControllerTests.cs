using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Server.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> authServiceMock;
    private Mock<IHttpContextAccessor> httpContextAccessorMock;
    private AuthController authController;

    [SetUp]
    public void Setup()
    {
        authServiceMock = new Mock<IAuthService>();
        httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        authController = new AuthController(authServiceMock.Object);

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        var authServiceMock2 = new Mock<IAuthenticationService>();
        httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock2.Object)
            .BuildServiceProvider();

        authController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    #region Login Tests

    [Test]
    public async Task Login_WithValidEmail_ReturnsOkResponse()
    {
        // Arrange
        var loginRequest = new LoginDTO { Email = "teacher@example.com" };
        var teacherLoginDto = new TeacherLoginDTO
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "teacher@example.com",
            TeacherCode = "T001"
        };

        var response = Response<TeacherLoginDTO>.Ok(teacherLoginDto);

        authServiceMock
            .Setup(s => s.LoginTeacherByEmail(loginRequest.Email))
            .ReturnsAsync(response);

        // Act
        var result = await authController.Login(loginRequest);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(teacherLoginDto));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(loginRequest.Email), Times.Once);
    }

    [Test]
    public async Task Login_WithNullRequest_ReturnsBadRequest()
    {
        // Act
        var result = await authController.Login(null!);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        var badResult = result as BadRequestObjectResult;
        Assert.That(badResult!.StatusCode, Is.EqualTo(400));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Login_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginDTO { Email = "" };

        // Act
        var result = await authController.Login(loginRequest);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        var badResult = result as BadRequestObjectResult;
        Assert.That(badResult!.StatusCode, Is.EqualTo(400));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Login_WithNonExistentEmail_ReturnsErrorResponse()
    {
        // Arrange
        var loginRequest = new LoginDTO { Email = "nonexistent@example.com" };
        var response = Response<TeacherLoginDTO>.NotFound("No teacher found with email");

        authServiceMock
            .Setup(s => s.LoginTeacherByEmail(loginRequest.Email))
            .ReturnsAsync(response);

        // Act
        var result = await authController.Login(loginRequest);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        authServiceMock.Verify(s => s.LoginTeacherByEmail(loginRequest.Email), Times.Once);
    }

    [Test]
    public async Task Login_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var loginRequest = new LoginDTO { Email = "teacher@example.com" };

        authServiceMock
            .Setup(s => s.LoginTeacherByEmail(loginRequest.Email))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => authController.Login(loginRequest));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(loginRequest.Email), Times.Once);
    }

    #endregion

    #region LoginByEmail Tests

    [Test]
    public async Task LoginByEmail_WithValidEmail_ReturnsOkResponse()
    {
        // Arrange
        var email = "teacher@example.com";
        var teacherLoginDto = new TeacherLoginDTO
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = email,
            TeacherCode = "T002"
        };

        var response = Response<TeacherLoginDTO>.Ok(teacherLoginDto);

        authServiceMock
            .Setup(s => s.LoginTeacherByEmail(email))
            .ReturnsAsync(response);

        // Act
        var result = await authController.LoginByEmail(email);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(teacherLoginDto));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(email), Times.Once);
    }

    [Test]
    public async Task LoginByEmail_WithNullEmail_ReturnsBadRequest()
    {
        // Act
        var result = await authController.LoginByEmail(null!);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        var badResult = result as BadRequestObjectResult;
        Assert.That(badResult!.StatusCode, Is.EqualTo(400));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task LoginByEmail_WithEmptyEmail_ReturnsBadRequest()
    {
        // Act
        var result = await authController.LoginByEmail("");

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        var badResult = result as BadRequestObjectResult;
        Assert.That(badResult!.StatusCode, Is.EqualTo(400));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task LoginByEmail_WithNonExistentEmail_ReturnsErrorResponse()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var response = Response<TeacherLoginDTO>.NotFound("No teacher found with email");

        authServiceMock
            .Setup(s => s.LoginTeacherByEmail(email))
            .ReturnsAsync(response);

        // Act
        var result = await authController.LoginByEmail(email);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        authServiceMock.Verify(s => s.LoginTeacherByEmail(email), Times.Once);
    }

    [Test]
    public async Task LoginByEmail_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var email = "teacher@example.com";

        authServiceMock
            .Setup(s => s.LoginTeacherByEmail(email))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => authController.LoginByEmail(email));
        authServiceMock.Verify(s => s.LoginTeacherByEmail(email), Times.Once);
    }

    #endregion

    #region Logout Tests

    [Test]
    public async Task Logout_AlwaysReturnsNoContent()
    {
        // Act
        var result = await authController.Logout();

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
        var noContentResult = result as NoContentResult;
        Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
    }

    [Test]
    public async Task Logout_WhenCalled_CompletesWithoutError()
    {
        // Act
        var result = await authController.Logout();

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    #endregion
}
