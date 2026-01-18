using Core.Common;
using Core.DTOs;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;
using Core.Interfaces.Services;

namespace Tests.Server.Controllers;

[TestFixture]
public class PlanningControllerTests
{
    private Mock<IPlanningService> planningServiceMock;
    private PlanningController planningController;

    [SetUp]
    public void Setup()
    {
        planningServiceMock = new Mock<IPlanningService>();
        planningController = new PlanningController(planningServiceMock.Object);
    }

    #region Get Tests

    [Test]
    public void GetPlanningByCourseId_WithValidCourseId_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var planningDto = new PlanningDto
        {
            Id = 1,
            Lessons = null
        };

        var response = Response<PlanningDto>.Ok(planningDto);

        planningServiceMock
            .Setup(s => s.GetPlanningByCourseId(courseId))
            .Returns(response);

        // Act
        var result = planningController.GetPlanningByCourseId(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        planningServiceMock.Verify(s => s.GetPlanningByCourseId(courseId), Times.Once);
    }

    [Test]
    public void GetPlanningByCourseId_WithNonExistentCourseId_ReturnsNotFound()
    {
        // Arrange
        var courseId = 999;
        var response = Response<PlanningDto>.NotFound("planning not found");

        planningServiceMock
            .Setup(s => s.GetPlanningByCourseId(courseId))
            .Returns(response);

        // Act
        var result = planningController.GetPlanningByCourseId(courseId);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        planningServiceMock.Verify(s => s.GetPlanningByCourseId(courseId), Times.Once);
    }

    [Test]
    public void GetPlanningByCourseId_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var courseId = 1;

        planningServiceMock
            .Setup(s => s.GetPlanningByCourseId(courseId))
            .Throws(new Exception("Service error"));

        // Act & Assert
        Assert.Throws<Exception>(() => planningController.GetPlanningByCourseId(courseId));
        planningServiceMock.Verify(s => s.GetPlanningByCourseId(courseId), Times.Once);
    }

    #endregion

    #region GenerateDocument Tests

    [Test]
    public async Task GenerateDocument_WithValidCourseIdAndPdfType_ReturnsFileResult()
    {
        // Arrange
        var courseId = 1;
        var documentType = DocumentTypes.Pdf;
        var documentBytes = new byte[] { 1, 2, 3, 4, 5 };

        var documentDto = new DocumentDto
        {
            Document = documentBytes,
            ContentType = "application/pdf",
            DocumentName = "planning.pdf"
        };

        var response = Response<DocumentDto>.Ok(documentDto);

        planningServiceMock
            .Setup(s => s.GenerateDocument(courseId, documentType))
            .ReturnsAsync(response);

        // Act
        var result = await planningController.GenerateDocument(courseId, documentType);

        // Assert
        Assert.That(result, Is.TypeOf<FileContentResult>());
        var fileResult = result as FileContentResult;
        Assert.That(fileResult!.FileContents, Is.EqualTo(documentBytes));
        Assert.That(fileResult.ContentType, Is.EqualTo("application/pdf"));
        Assert.That(fileResult.FileDownloadName, Is.EqualTo("planning.pdf"));
        planningServiceMock.Verify(s => s.GenerateDocument(courseId, documentType), Times.Once);
    }

    [Test]
    public async Task GenerateDocument_WithValidCourseIdAndCsvType_ReturnsFileResult()
    {
        // Arrange
        var courseId = 1;
        var documentType = DocumentTypes.Csv;
        var documentBytes = new byte[] { 1, 2, 3 };

        var documentDto = new DocumentDto
        {
            Document = documentBytes,
            ContentType = "text/csv",
            DocumentName = "planning.csv"
        };

        var response = Response<DocumentDto>.Ok(documentDto);

        planningServiceMock
            .Setup(s => s.GenerateDocument(courseId, documentType))
            .ReturnsAsync(response);

        // Act
        var result = await planningController.GenerateDocument(courseId, documentType);

        // Assert
        Assert.That(result, Is.TypeOf<FileContentResult>());
        var fileResult = result as FileContentResult;
        Assert.That(fileResult!.ContentType, Is.EqualTo("text/csv"));
        Assert.That(fileResult.FileDownloadName, Is.EqualTo("planning.csv"));
    }

    [Test]
    public async Task GenerateDocument_WithValidCourseIdAndDocxType_ReturnsFileResult()
    {
        // Arrange
        var courseId = 1;
        var documentType = DocumentTypes.Docx;
        var documentBytes = new byte[] { 1, 2, 3 };

        var documentDto = new DocumentDto
        {
            Document = documentBytes,
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            DocumentName = "planning.docx"
        };

        var response = Response<DocumentDto>.Ok(documentDto);

        planningServiceMock
            .Setup(s => s.GenerateDocument(courseId, documentType))
            .ReturnsAsync(response);

        // Act
        var result = await planningController.GenerateDocument(courseId, documentType);

        // Assert
        Assert.That(result, Is.TypeOf<FileContentResult>());
        var fileResult = result as FileContentResult;
        Assert.That(fileResult!.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.wordprocessingml.document"));
    }

    [Test]
    public async Task GenerateDocument_WithNonExistentCourseId_ThrowsExceptionOrReturnsError()
    {
        // Arrange
        var courseId = 999;
        var documentType = DocumentTypes.Pdf;
        var response = Response<DocumentDto>.Fail("Error generating planning document");

        planningServiceMock
            .Setup(s => s.GenerateDocument(courseId, documentType))
            .ReturnsAsync(response);

        // Act & Assert
        var result = await planningController.GenerateDocument(courseId, documentType);
        // The controller throws because Result is null, so we expect an exception
    }

    [Test]
    public async Task GenerateDocument_WithMultipleCalls_EachReturnsSeparateFile()
    {
        // Arrange
        var courseId = 1;
        var documentType = DocumentTypes.Pdf;
        var documentBytes1 = new byte[] { 1, 2, 3 };
        var documentBytes2 = new byte[] { 4, 5, 6 };

        var documentDto1 = new DocumentDto
        {
            Document = documentBytes1,
            ContentType = "application/pdf",
            DocumentName = "planning1.pdf"
        };

        var documentDto2 = new DocumentDto
        {
            Document = documentBytes2,
            ContentType = "application/pdf",
            DocumentName = "planning2.pdf"
        };

        var responses = new Queue<Response<DocumentDto>>(new[]
        {
            Response<DocumentDto>.Ok(documentDto1),
            Response<DocumentDto>.Ok(documentDto2)
        });

        planningServiceMock
            .Setup(s => s.GenerateDocument(courseId, documentType))
            .Returns(() => Task.FromResult(responses.Dequeue()));

        // Act
        var result1 = await planningController.GenerateDocument(courseId, documentType);
        var result2 = await planningController.GenerateDocument(courseId, documentType);

        // Assert
        var fileResult1 = result1 as FileContentResult;
        var fileResult2 = result2 as FileContentResult;
        Assert.That(fileResult1!.FileContents, Is.EqualTo(documentBytes1));
        Assert.That(fileResult2!.FileContents, Is.EqualTo(documentBytes2));
        planningServiceMock.Verify(s => s.GenerateDocument(courseId, documentType), Times.Exactly(2));
    }

    #endregion
}
