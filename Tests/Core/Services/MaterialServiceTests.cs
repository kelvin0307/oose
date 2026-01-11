using Core.DTOs;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.Services;
using Domain.Models;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using Core.Interfaces.Repositories;

namespace Tests.Core.Services;

[TestFixture]
public class MaterialServiceTests
{
    private Mock<IDocumentFactory> documentFactoryMock;
    private Mock<IRepository<Material>> materialRepositoryMock;
    private MaterialService materialService;

    [SetUp]
    public void Setup()
    {
        documentFactoryMock = new Mock<IDocumentFactory>();
        materialRepositoryMock = new Mock<IRepository<Material>>();
        materialService = new MaterialService(documentFactoryMock.Object, materialRepositoryMock.Object);
    }

    #region GenerateDocument Tests

    [Test]
    public async Task GenerateDocument_WithValidMaterialId_ReturnsOkResponse()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Pdf;
        
        var material = new Material
        {
            Id = materialId,
            Name = "Test Material",
            Content = "This is test content"
        };

        var documentBytes = new byte[] { 1, 2, 3, 4, 5 };

        materialRepositoryMock
            .Setup(r => r.Get(materialId))
            .ReturnsAsync(material);

        documentFactoryMock
            .Setup(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), documentType))
            .Returns(new DocumentDTO
            {
                Document = documentBytes,
                ContentType = "application/pdf",
                DocumentName = "material.pdf"
            });

        // Act
        var result = await materialService.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Document, Is.EqualTo(documentBytes));
        Assert.That(result.Result.ContentType, Is.EqualTo("application/pdf"));
        materialRepositoryMock.Verify(r => r.Get(materialId), Times.Once);
    }

    [Test]
    public async Task GenerateDocument_WithNonExistentMaterialId_ReturnsFail()
    {
        // Arrange
        var materialId = 999;
        var documentType = DocumentTypes.Pdf;

        materialRepositoryMock
            .Setup(r => r.Get(materialId))
            .ReturnsAsync((Material)null!);

        // Act
        var result = await materialService.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error generating material document"));
        materialRepositoryMock.Verify(r => r.Get(materialId), Times.Once);
    }

    [Test]
    public async Task GenerateDocument_WhenRepositoryThrowsException_ReturnsFail()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Pdf;

        materialRepositoryMock
            .Setup(r => r.Get(materialId))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await materialService.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error generating material document"));
        materialRepositoryMock.Verify(r => r.Get(materialId), Times.Once);
    }

    [Test]
    public async Task GenerateDocument_WithDifferentDocumentTypes_CreatesCorrectDocument()
    {
        // Arrange
        var materialId = 1;
        var documentTypes = new[] { DocumentTypes.Pdf, DocumentTypes.Csv, DocumentTypes.Docx };
        
        var material = new Material
        {
            Id = materialId,
            Name = "Test Material",
            Content = "This is test content"
        };

        materialRepositoryMock
            .Setup(r => r.Get(materialId))
            .ReturnsAsync(material);

        documentFactoryMock
            .Setup(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), It.IsAny<DocumentTypes>()))
            .Returns((DocumentDataDTO data, DocumentTypes type) => new DocumentDTO
            {
                Document = new byte[] { 1, 2, 3 },
                ContentType = "application/octet-stream",
                DocumentName = $"material.{type.ToString().ToLower()}"
            });

        // Act & Assert
        foreach (var docType in documentTypes)
        {
            var result = await materialService.GenerateDocument(materialId, docType);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Result.DocumentName, Does.Contain(docType.ToString().ToLower()));
        }

        documentFactoryMock.Verify(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), It.IsAny<DocumentTypes>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task GenerateDocument_WithLargeMaterialContent_ReturnsOkResponse()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Pdf;
        var largeContent = string.Concat(Enumerable.Repeat("This is large content. ", 1000));
        
        var material = new Material
        {
            Id = materialId,
            Name = "Large Material",
            Content = largeContent
        };

        var documentBytes = new byte[] { 1, 2, 3, 4, 5 };

        materialRepositoryMock
            .Setup(r => r.Get(materialId))
            .ReturnsAsync(material);

        documentFactoryMock
            .Setup(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), documentType))
            .Returns(new DocumentDTO
            {
                Document = documentBytes,
                ContentType = "application/pdf",
                DocumentName = "material.pdf"
            });

        // Act
        var result = await materialService.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Document.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task GenerateDocument_WithEmptyContent_ReturnsOkResponse()
    {
        // Arrange
        var materialId = 1;
        var documentType = DocumentTypes.Pdf;
        
        var material = new Material
        {
            Id = materialId,
            Name = "Empty Material",
            Content = ""
        };

        var documentBytes = new byte[] { 1, 2, 3 };

        materialRepositoryMock
            .Setup(r => r.Get(materialId))
            .ReturnsAsync(material);

        documentFactoryMock
            .Setup(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), documentType))
            .Returns(new DocumentDTO
            {
                Document = documentBytes,
                ContentType = "application/pdf",
                DocumentName = "material.pdf"
            });

        // Act
        var result = await materialService.GenerateDocument(materialId, documentType);

        // Assert
        Assert.That(result.Success, Is.True);
        documentFactoryMock.Verify(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), documentType), Times.Once);
    }

    #endregion
}
