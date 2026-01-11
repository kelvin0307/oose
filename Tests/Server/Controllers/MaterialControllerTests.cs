using Core.Common;
using Core.DTOs;
using Core.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Server.Controllers;

namespace Tests.Server.Controllers;

[TestFixture]
public class MaterialControllerTests
{
    private Mock<IMaterialService> materialServiceMock;
    private MaterialController materialController;

    [SetUp]
    public void Setup()
    {
        materialServiceMock = new Mock<IMaterialService>();
        materialController = new MaterialController(materialServiceMock.Object);
    }

    #region GenerateDocument Tests

    [Test]
    public async Task GenerateDocument_WithValidMaterialIdAndPdfType_ReturnsFileResult()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Pdf;
        var documentBytes = new byte[] { 1, 2, 3, 4, 5 };

        var documentDto = new DocumentDTO
        {
            Document = documentBytes,
            ContentType = "application/pdf",
            DocumentName = "material.pdf"
        };

        var response = Response<DocumentDTO>.Ok(documentDto);

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId, documentType))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result, Is.TypeOf<FileContentResult>());
        var fileResult = result as FileContentResult;
        Assert.That(fileResult!.FileContents, Is.EqualTo(documentBytes));
        Assert.That(fileResult.ContentType, Is.EqualTo("application/pdf"));
        Assert.That(fileResult.FileDownloadName, Is.EqualTo("material.pdf"));
        materialServiceMock.Verify(s => s.GenerateDocument(materialId, documentType), Times.Once);
    }

    [Test]
    public async Task GenerateDocument_WithValidMaterialIdAndCsvType_ReturnsFileResult()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Csv;
        var documentBytes = new byte[] { 1, 2, 3 };

        var documentDto = new DocumentDTO
        {
            Document = documentBytes,
            ContentType = "text/csv",
            DocumentName = "material.csv"
        };

        var response = Response<DocumentDTO>.Ok(documentDto);

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId, documentType))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result, Is.TypeOf<FileContentResult>());
        var fileResult = result as FileContentResult;
        Assert.That(fileResult!.ContentType, Is.EqualTo("text/csv"));
        Assert.That(fileResult.FileDownloadName, Is.EqualTo("material.csv"));
        materialServiceMock.Verify(s => s.GenerateDocument(materialId, documentType), Times.Once);
    }

    [Test]
    public async Task GenerateDocument_WithValidMaterialIdAndDocxType_ReturnsFileResult()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Docx;
        var documentBytes = new byte[] { 1, 2, 3 };

        var documentDto = new DocumentDTO
        {
            Document = documentBytes,
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            DocumentName = "material.docx"
        };

        var response = Response<DocumentDTO>.Ok(documentDto);

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId, documentType))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result, Is.TypeOf<FileContentResult>());
        var fileResult = result as FileContentResult;
        Assert.That(fileResult!.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.wordprocessingml.document"));
        Assert.That(fileResult.FileDownloadName, Is.EqualTo("material.docx"));
    }

    [Test]
    public async Task GenerateDocument_WithNonExistentMaterialId_ThrowsExceptionOrReturnsError()
    {
        // Arrange
        var materialId = 999;
        var documentType = DocumentTypes.Pdf;
        var response = Response<DocumentDTO>.Fail("Error generating material document");

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId, documentType))
            .ReturnsAsync(response);

        // Act 
        var result = await materialController.GenerateDocument(materialId, documentType);

        //Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        materialServiceMock.Verify(s => s.GenerateDocument(materialId, documentType), Times.Once);
    }

    [Test]
    public async Task GenerateDocument_WithDifferentMaterialIds_ReturnsCorrectDocuments()
    {
        // Arrange
        var materialIds = new[] { 1, 2, 3 };
        var documentType = DocumentTypes.Pdf;

        foreach (var id in materialIds)
        {
            var documentBytes = new byte[] { (byte)id, 2, 3 };
            var documentDto = new DocumentDTO
            {
                Document = documentBytes,
                ContentType = "application/pdf",
                DocumentName = $"material{id}.pdf"
            };

            materialServiceMock
                .Setup(s => s.GenerateDocument(id, documentType))
                .ReturnsAsync(Response<DocumentDTO>.Ok(documentDto));
        }

        // Act & Assert
        foreach (var id in materialIds)
        {
            var result = await materialController.GenerateDocument(id, documentType);
            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult!.FileDownloadName, Is.EqualTo($"material{id}.pdf"));
        }
    }

    [Test]
    public async Task GenerateDocument_WithLargeDocument_ReturnsFileWithCorrectSize()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Pdf;
        var documentBytes = new byte[10000]; // 10KB document
        for (int i = 0; i < documentBytes.Length; i++)
        {
            documentBytes[i] = (byte)(i % 256);
        }

        var documentDto = new DocumentDTO
        {
            Document = documentBytes,
            ContentType = "application/pdf",
            DocumentName = "large_material.pdf"
        };

        var response = Response<DocumentDTO>.Ok(documentDto);

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId, documentType))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result, Is.TypeOf<FileContentResult>());
        var fileResult = result as FileContentResult;
        Assert.That(fileResult!.FileContents.Length, Is.EqualTo(10000));
    }

    [Test]
    public async Task GenerateDocument_WithMultipleCalls_EachReturnsSeparateFile()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Pdf;
        var documentBytes1 = new byte[] { 1, 2, 3 };
        var documentBytes2 = new byte[] { 4, 5, 6 };

        var documentDto1 = new DocumentDTO
        {
            Document = documentBytes1,
            ContentType = "application/pdf",
            DocumentName = "material1.pdf"
        };

        var documentDto2 = new DocumentDTO
        {
            Document = documentBytes2,
            ContentType = "application/pdf",
            DocumentName = "material2.pdf"
        };

        var responses = new Queue<Response<DocumentDTO>>(new[]
        {
            Response<DocumentDTO>.Ok(documentDto1),
            Response<DocumentDTO>.Ok(documentDto2)
        });

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId, documentType))
            .Returns(() => Task.FromResult(responses.Dequeue()));

        // Act
        var result1 = await materialController.GenerateDocument(materialId, documentType);
        var result2 = await materialController.GenerateDocument(materialId, documentType);

        // Assert
        var fileResult1 = result1 as FileContentResult;
        var fileResult2 = result2 as FileContentResult;
        Assert.That(fileResult1!.FileContents, Is.EqualTo(documentBytes1));
        Assert.That(fileResult2!.FileContents, Is.EqualTo(documentBytes2));
        materialServiceMock.Verify(s => s.GenerateDocument(materialId, documentType), Times.Exactly(2));
    }

    #endregion
}
