using AutoMapper;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Services;
using Domain.Enums;
using Domain.Models;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace Tests.Core.Services;

[TestFixture]
public class MaterialServiceTests
{
    private Mock<IDocumentFactory> documentFactoryMock;
    private Mock<IRepository<Material>> materialRepositoryMock;
    private Mock<IRepository<Lesson>> lessonRepositoryMock;
    private Mock<IMapper> mapperMock;
    private MaterialService materialService;

    [SetUp]
    public void Setup()
    {
        documentFactoryMock = new Mock<IDocumentFactory>();
        materialRepositoryMock = new Mock<IRepository<Material>>();
        lessonRepositoryMock = new Mock<IRepository<Lesson>>();
        mapperMock = new Mock<IMapper>();
        materialService = new MaterialService(documentFactoryMock.Object, materialRepositoryMock.Object, lessonRepositoryMock.Object, mapperMock.Object);
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

    #region CreateMaterial Tests

    [Test]
    public async Task CreateMaterial_WithValidData_ReturnsOkResponse()
    {
        // Arrange
        var createMaterialDTO = new CreateMaterialDTO
        {
            Name = "New Material",
            Content = "Material content",
            LessonId = 1
        };

        var lesson = new Lesson
        {
            Id = 1,
            Name = "Lesson 1",
            WeekNumber = 1,
            SequenceNumber = 1
        };

        var materials = new List<Material>
        {
            new Material { Id = 0, Name = "Test", Content = "Test", Version = 1, LessonId = 1 }
        }.AsQueryable();

        var createdMaterial = new Material
        {
            Id = 1,
            Name = createMaterialDTO.Name,
            Content = createMaterialDTO.Content,
            Version = 1,
            LessonId = lesson.Id
        };

        var expectedDTO = new MaterialDTO
        {
            Id = 1,
            Name = "New Material",
            Content = "Material content",
            Version = 1
        };

        lessonRepositoryMock
            .Setup(r => r.Get(createMaterialDTO.LessonId))
            .ReturnsAsync(lesson);

        materialRepositoryMock
            .Setup(r => r.OrderByDescending(It.IsAny<Expression<Func<Material, int>>>()))
            .Returns(materials.OrderByDescending(m => m.Id));

        materialRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Material>()))
            .ReturnsAsync(createdMaterial);

        mapperMock
            .Setup(m => m.Map<MaterialDTO>(createdMaterial))
            .Returns(expectedDTO);

        // Act
        var result = await materialService.CreateMaterial(createMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Name, Is.EqualTo("New Material"));
        Assert.That(result.Result.Version, Is.EqualTo(1));
        lessonRepositoryMock.Verify(r => r.Get(createMaterialDTO.LessonId), Times.Once);
    }

    [Test]
    public async Task CreateMaterial_WithNonExistentLesson_ReturnsFail()
    {
        // Arrange
        var createMaterialDTO = new CreateMaterialDTO
        {
            Name = "New Material",
            Content = "Material content",
            LessonId = 999
        };

        lessonRepositoryMock
            .Setup(r => r.Get(createMaterialDTO.LessonId))
            .ReturnsAsync((Lesson)null!);

        // Act
        var result = await materialService.CreateMaterial(createMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Lesson not found"));
        lessonRepositoryMock.Verify(r => r.Get(createMaterialDTO.LessonId), Times.Once);
    }

    [Test]
    public async Task CreateMaterial_WhenRepositoryThrowsException_ReturnsFail()
    {
        // Arrange
        var createMaterialDTO = new CreateMaterialDTO
        {
            Name = "New Material",
            Content = "Material content",
            LessonId = 1
        };

        var lesson = new Lesson
        {
            Id = 1,
            Name = "Lesson 1",
            WeekNumber = 1,
            SequenceNumber = 1
        };

        lessonRepositoryMock
            .Setup(r => r.Get(createMaterialDTO.LessonId))
            .ReturnsAsync(lesson);

        materialRepositoryMock
            .Setup(r => r.OrderByDescending(It.IsAny<Expression<Func<Material, int>>>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = await materialService.CreateMaterial(createMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error creating material"));
    }

    [Test]
    public async Task CreateMaterial_WithEmptyName_StillCreates()
    {
        // Arrange
        var createMaterialDTO = new CreateMaterialDTO
        {
            Name = "",
            Content = "Material content",
            LessonId = 1
        };

        var lesson = new Lesson
        {
            Id = 1,
            Name = "Lesson 1",
            WeekNumber = 1,
            SequenceNumber = 1
        };

        var materials = new List<Material>().AsQueryable();

        var createdMaterial = new Material
        {
            Id = 1,
            Name = "",
            Content = "Material content",
            Version = 1,
            LessonId = lesson.Id
        };

        var expectedDTO = new MaterialDTO
        {
            Id = 1,
            Name = "",
            Content = "Material content",
            Version = 1
        };

        lessonRepositoryMock
            .Setup(r => r.Get(createMaterialDTO.LessonId))
            .ReturnsAsync(lesson);

        materialRepositoryMock
            .Setup(r => r.OrderByDescending(It.IsAny<Expression<Func<Material, int>>>()))
            .Returns(materials.OrderByDescending(m => m.Id));

        materialRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Material>()))
            .ReturnsAsync(createdMaterial);

        mapperMock
            .Setup(m => m.Map<MaterialDTO>(createdMaterial))
            .Returns(expectedDTO);

        // Act
        var result = await materialService.CreateMaterial(createMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.True);
    }

    #endregion

    #region UpdateMaterial Tests

    [Test]
    public async Task UpdateMaterial_WithValidData_ReturnsOkResponse()
    {
        // Arrange
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = 1,
            Version = 1,
            Name = "Updated Material",
            Content = "Updated content"
        };

        var currentMaterial = new Material
        {
            Id = 1,
            Name = "Old Name",
            Content = "Old content",
            Version = 1,
            LessonId = 1
        };

        var updatedMaterial = new Material
        {
            Id = 1,
            Name = "Updated Material",
            Content = "Updated content",
            Version = 2,
            LessonId = 1
        };

        var expectedDTO = new MaterialDTO
        {
            Id = 1,
            Name = "Updated Material",
            Content = "Updated content",
            Version = 2
        };

        materialRepositoryMock
            .Setup(r => r.Find(It.IsAny<Expression<Func<Material, bool>>>()))
            .Returns(new List<Material> { currentMaterial }.AsQueryable());

        materialRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Material>()))
            .ReturnsAsync(updatedMaterial);

        mapperMock
            .Setup(m => m.Map<MaterialDTO>(updatedMaterial))
            .Returns(expectedDTO);

        // Act
        var result = await materialService.UpdateMaterial(updateMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Name, Is.EqualTo("Updated Material"));
        Assert.That(result.Result.Version, Is.EqualTo(2));
        materialRepositoryMock.Verify(r => r.CreateAndCommit(It.IsAny<Material>()), Times.Once);
    }

    [Test]
    public async Task UpdateMaterial_WithNonExistentMaterial_ReturnsFail()
    {
        // Arrange
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = 999,
            Version = 1,
            Name = "Updated Material",
            Content = "Updated content"
        };

        materialRepositoryMock
            .Setup(r => r.Find(It.IsAny<Expression<Func<Material, bool>>>()))
            .Returns(new List<Material>().AsQueryable());

        // Act
        var result = await materialService.UpdateMaterial(updateMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Material not found"));
    }

    [Test]
    public async Task UpdateMaterial_WithIncorrectVersion_ReturnsFail()
    {
        // Arrange
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = 1,
            Version = 1,
            Name = "Updated Material",
            Content = "Updated content"
        };

        // Current material has version 2, but update requests version 1
        var currentMaterial = new Material
        {
            Id = 1,
            Name = "Old Name",
            Content = "Old content",
            Version = 2,
            LessonId = 1
        };

        materialRepositoryMock
            .Setup(r => r.Find(It.IsAny<Expression<Func<Material, bool>>>()))
            .Returns(new List<Material>().AsQueryable()); // Empty because version mismatch

        // Act
        var result = await materialService.UpdateMaterial(updateMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Material not found"));
    }

    [Test]
    public async Task UpdateMaterial_WhenRepositoryThrowsException_ReturnsFail()
    {
        // Arrange
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = 1,
            Version = 1,
            Name = "Updated Material",
            Content = "Updated content"
        };

        var currentMaterial = new Material
        {
            Id = 1,
            Name = "Old Name",
            Content = "Old content",
            Version = 1,
            LessonId = 1
        };

        materialRepositoryMock
            .Setup(r => r.Find(It.IsAny<Expression<Func<Material, bool>>>()))
            .Returns(new List<Material> { currentMaterial }.AsQueryable());

        materialRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Material>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await materialService.UpdateMaterial(updateMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error updating material"));
    }

    [Test]
    public async Task UpdateMaterial_IncrementVersionCorrectly()
    {
        // Arrange
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = 1,
            Version = 5,
            Name = "Updated Material",
            Content = "Updated content"
        };

        var currentMaterial = new Material
        {
            Id = 1,
            Name = "Old Name",
            Content = "Old content",
            Version = 5,
            LessonId = 1
        };

        var updatedMaterial = new Material
        {
            Id = 1,
            Name = "Updated Material",
            Content = "Updated content",
            Version = 6,
            LessonId = 1
        };

        var expectedDTO = new MaterialDTO
        {
            Id = 1,
            Name = "Updated Material",
            Content = "Updated content",
            Version = 6
        };

        materialRepositoryMock
            .Setup(r => r.Find(It.IsAny<Expression<Func<Material, bool>>>()))
            .Returns(new List<Material> { currentMaterial }.AsQueryable());

        materialRepositoryMock
            .Setup(r => r.CreateAndCommit(It.IsAny<Material>()))
            .ReturnsAsync(updatedMaterial);

        mapperMock
            .Setup(m => m.Map<MaterialDTO>(updatedMaterial))
            .Returns(expectedDTO);

        // Act
        var result = await materialService.UpdateMaterial(updateMaterialDTO);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Version, Is.EqualTo(6));
    }

    #endregion

    #region GetMaterialByLessonId Tests

    [Test]
    public async Task GetMaterialByLessonId_WithValidLessonId_ReturnsOkResponse()
    {
        // Arrange
        var lessonId = 1;
        var materials = new List<Material>
        {
            new Material
            {
                Id = 1,
                Name = "Material 1",
                Content = "Content 1",
                Version = 1,
                LessonId = lessonId,
                SysDeleted = null
            },
            new Material
            {
                Id = 2,
                Name = "Material 2",
                Content = "Content 2",
                Version = 1,
                LessonId = lessonId,
                SysDeleted = null
            }
        };

        var expectedDTOs = new List<MaterialDTO>
        {
            new MaterialDTO { Id = 1, Name = "Material 1", Content = "Content 1", Version = 1 },
            new MaterialDTO { Id = 2, Name = "Material 2", Content = "Content 2", Version = 1 }
        };

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        mapperMock
            .Setup(m => m.Map<IList<MaterialDTO>>(It.IsAny<List<Material>>()))
            .Returns(expectedDTOs);

        // Act
        var result = await materialService.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Count, Is.EqualTo(2));
        materialRepositoryMock.Verify(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
    }

    [Test]
    public async Task GetMaterialByLessonId_WithNoMaterials_ReturnsEmptyList()
    {
        // Arrange
        var lessonId = 999;
        var materials = new List<Material>();

        var expectedDTOs = new List<MaterialDTO>();

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        mapperMock
            .Setup(m => m.Map<IList<MaterialDTO>>(It.IsAny<List<Material>>()))
            .Returns(expectedDTOs);

        // Act
        var result = await materialService.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetMaterialByLessonId_ExcludesDeletedMaterials()
    {
        // Arrange
        var lessonId = 1;
        var materials = new List<Material>
        {
            new Material
            {
                Id = 1,
                Name = "Material 1",
                Content = "Content 1",
                Version = 1,
                LessonId = lessonId,
                SysDeleted = null
            }
        };

        var expectedDTOs = new List<MaterialDTO>
        {
            new MaterialDTO { Id = 1, Name = "Material 1", Content = "Content 1", Version = 1 }
        };

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        mapperMock
            .Setup(m => m.Map<IList<MaterialDTO>>(It.IsAny<List<Material>>()))
            .Returns(expectedDTOs);

        // Act
        var result = await materialService.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task GetMaterialByLessonId_WithMultipleVersions_ReturnsLatestVersions()
    {
        // Arrange
        var lessonId = 1;
        var materials = new List<Material>
        {
            new Material { Id = 1, Name = "Material 1 V1", Content = "Content 1", Version = 1, LessonId = lessonId, SysDeleted = null },
            new Material { Id = 1, Name = "Material 1 V2", Content = "Content 1 Updated", Version = 2, LessonId = lessonId, SysDeleted = null },
            new Material { Id = 2, Name = "Material 2 V1", Content = "Content 2", Version = 1, LessonId = lessonId, SysDeleted = null }
        };

        var expectedDTOs = new List<MaterialDTO>
        {
            new MaterialDTO { Id = 1, Name = "Material 1 V2", Content = "Content 1 Updated", Version = 2 },
            new MaterialDTO { Id = 2, Name = "Material 2 V1", Content = "Content 2", Version = 1 }
        };

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        mapperMock
            .Setup(m => m.Map<IList<MaterialDTO>>(It.IsAny<List<Material>>()))
            .Returns(expectedDTOs);

        // Act
        var result = await materialService.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task GetMaterialByLessonId_WhenRepositoryThrowsException_ReturnsFail()
    {
        // Arrange
        var lessonId = 1;

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await materialService.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error retrieving material"));
    }

    #endregion

    #region DeleteMaterial Tests

    [Test]
    public async Task DeleteMaterial_WithValidMaterialId_ReturnsTrue()
    {
        // Arrange
        var materialId = 1;
        var materials = new List<Material>
        {
            new Material
            {
                Id = materialId,
                Name = "Material to Delete",
                Content = "Content",
                Version = 1,
                LessonId = 1,
                SysDeleted = null
            }
        };

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        materialRepositoryMock
            .Setup(r => r.Update(It.IsAny<Material>()))
            .Callback<Material>(m => m.SysDeleted = DateTimeOffset.UtcNow);

        materialRepositoryMock
            .Setup(r => r.SaveManually())
            .Returns(Task.CompletedTask);

        // Act
        var result = await materialService.DeleteMaterial(materialId);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.True);
        materialRepositoryMock.Verify(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
        materialRepositoryMock.Verify(r => r.Update(It.IsAny<Material>()), Times.Once);
        materialRepositoryMock.Verify(r => r.SaveManually(), Times.Once);
    }

    [Test]
    public async Task DeleteMaterial_WithNonExistentMaterialId_StillReturnsTrue()
    {
        // Arrange
        var materialId = 999;
        var materials = new List<Material>();

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        materialRepositoryMock
            .Setup(r => r.SaveManually())
            .Returns(Task.CompletedTask);

        // Act
        var result = await materialService.DeleteMaterial(materialId);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.True);
        materialRepositoryMock.Verify(r => r.Update(It.IsAny<Material>()), Times.Never);
    }

    [Test]
    public async Task DeleteMaterial_WithMultipleVersions_DeletesAllVersions()
    {
        // Arrange
        var materialId = 1;
        var materials = new List<Material>
        {
            new Material { Id = materialId, Name = "Material V1", Content = "Content", Version = 1, LessonId = 1, SysDeleted = null },
            new Material { Id = materialId, Name = "Material V2", Content = "Updated Content", Version = 2, LessonId = 1, SysDeleted = null }
        };

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        materialRepositoryMock
            .Setup(r => r.Update(It.IsAny<Material>()))
            .Callback<Material>(m => m.SysDeleted = DateTimeOffset.UtcNow);

        materialRepositoryMock
            .Setup(r => r.SaveManually())
            .Returns(Task.CompletedTask);

        // Act
        var result = await materialService.DeleteMaterial(materialId);

        // Assert
        Assert.That(result.Success, Is.True);
        materialRepositoryMock.Verify(r => r.Update(It.IsAny<Material>()), Times.Exactly(2));
    }

    [Test]
    public async Task DeleteMaterial_WhenRepositoryThrowsException_ReturnsFail()
    {
        // Arrange
        var materialId = 1;

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await materialService.DeleteMaterial(materialId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error retrieving material"));
    }

    [Test]
    public async Task DeleteMaterial_WhenSaveThrowsException_ReturnsFail()
    {
        // Arrange
        var materialId = 1;
        var materials = new List<Material>
        {
            new Material { Id = materialId, Name = "Material", Content = "Content", Version = 1, LessonId = 1, SysDeleted = null }
        };

        materialRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Material, bool>>>()))
            .ReturnsAsync(materials);

        materialRepositoryMock
            .Setup(r => r.SaveManually())
            .ThrowsAsync(new Exception("Save error"));

        // Act
        var result = await materialService.DeleteMaterial(materialId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error retrieving material"));
    }

    #endregion
}
