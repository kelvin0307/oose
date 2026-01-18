using Core.Common;
using Core.DTOs;
using Core.Interfaces.Services;
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
        var materialId = new MaterialIdDTO() { MaterialId = 1, Version = 1 };
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
        var materialId = new MaterialIdDTO() { MaterialId = 1, Version = 1 };
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
        var materialId = new MaterialIdDTO() { MaterialId = 1, Version = 1 };
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
        var materialId = new MaterialIdDTO() { MaterialId = 999, Version = 1 };
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
        var documentType = DocumentTypes.Pdf;

        var materialId1 = new MaterialIdDTO() { MaterialId = 1, Version = 1 };
        var materialId2 = new MaterialIdDTO() { MaterialId = 2, Version = 1 };
        var materialId3 = new MaterialIdDTO() { MaterialId = 3, Version = 1 };

        var documentDto1 = new DocumentDTO
        {
            Document = new byte[] { 1, 2, 3 },
            ContentType = "application/pdf",
            DocumentName = "material1.pdf"
        };

        var documentDto2 = new DocumentDTO
        {
            Document = new byte[] { 2, 2, 3 },
            ContentType = "application/pdf",
            DocumentName = "material2.pdf"
        };

        var documentDto3 = new DocumentDTO
        {
            Document = new byte[] { 3, 2, 3 },
            ContentType = "application/pdf",
            DocumentName = "material3.pdf"
        };

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId1, documentType))
            .ReturnsAsync(Response<DocumentDTO>.Ok(documentDto1));

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId2, documentType))
            .ReturnsAsync(Response<DocumentDTO>.Ok(documentDto2));

        materialServiceMock
            .Setup(s => s.GenerateDocument(materialId3, documentType))
            .ReturnsAsync(Response<DocumentDTO>.Ok(documentDto3));

        // Act & Assert
        var result1 = await materialController.GenerateDocument(materialId1, documentType);
        var result2 = await materialController.GenerateDocument(materialId2, documentType);
        var result3 = await materialController.GenerateDocument(materialId3, documentType);

        Assert.That(result1, Is.TypeOf<FileContentResult>());
        Assert.That(result2, Is.TypeOf<FileContentResult>());
        Assert.That(result3, Is.TypeOf<FileContentResult>());

        var fileResult1 = result1 as FileContentResult;
        var fileResult2 = result2 as FileContentResult;
        var fileResult3 = result3 as FileContentResult;

        Assert.That(fileResult1!.FileDownloadName, Is.EqualTo("material1.pdf"));
        Assert.That(fileResult2!.FileDownloadName, Is.EqualTo("material2.pdf"));
        Assert.That(fileResult3!.FileDownloadName, Is.EqualTo("material3.pdf"));
    }

    [Test]
    public async Task GenerateDocument_WithLargeDocument_ReturnsFileWithCorrectSize()
    {
        // Arrange
        var materialId = new MaterialIdDTO() { MaterialId = 1, Version = 1 };
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
        var materialId = new MaterialIdDTO() { MaterialId = 1, Version = 1 };
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

        var createdMaterialDTO = new MaterialDTO
        {
            Id = 1,
            Name = "New Material",
            Content = "Material content",
            Version = 1
        };

        var response = Response<MaterialDTO>.Ok(createdMaterialDTO);

        materialServiceMock
            .Setup(s => s.CreateMaterial(createMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.CreateMaterial(createMaterialDTO);

        // Assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var okResult = result as CreatedAtActionResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(201));
        Assert.That(okResult.Value, Is.EqualTo(createdMaterialDTO));
        materialServiceMock.Verify(s => s.CreateMaterial(createMaterialDTO), Times.Once);
    }

    [Test]
    public async Task CreateMaterial_WithInvalidLesson_ReturnsBadRequest()
    {
        // Arrange
        var createMaterialDTO = new CreateMaterialDTO
        {
            Name = "New Material",
            Content = "Material content",
            LessonId = 999
        };

        var response = Response<MaterialDTO>.Fail("Lesson not found");

        materialServiceMock
            .Setup(s => s.CreateMaterial(createMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.CreateMaterial(createMaterialDTO);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        materialServiceMock.Verify(s => s.CreateMaterial(createMaterialDTO), Times.Once);
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

        var createdMaterialDTO = new MaterialDTO
        {
            Id = 1,
            Name = "",
            Content = "Material content",
            Version = 1
        };

        var response = Response<MaterialDTO>.Ok(createdMaterialDTO);

        materialServiceMock
            .Setup(s => s.CreateMaterial(createMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.CreateMaterial(createMaterialDTO);

        // Assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var okResult = result as CreatedAtActionResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(201));
        Assert.That(okResult!.Value, Is.EqualTo(createdMaterialDTO));

    }

    [Test]
    public async Task CreateMaterial_WithMultipleMaterialsInLesson_CreatesCorrectly()
    {
        // Arrange
        var createMaterialDTO1 = new CreateMaterialDTO
        {
            Name = "Material 1",
            Content = "Content 1",
            LessonId = 1
        };

        var createMaterialDTO2 = new CreateMaterialDTO
        {
            Name = "Material 2",
            Content = "Content 2",
            LessonId = 1
        };

        var createdMaterialDTO1 = new MaterialDTO { Id = 1, Name = "Material 1", Content = "Content 1", Version = 1 };
        var createdMaterialDTO2 = new MaterialDTO { Id = 2, Name = "Material 2", Content = "Content 2", Version = 1 };

        materialServiceMock
            .Setup(s => s.CreateMaterial(createMaterialDTO1))
            .ReturnsAsync(Response<MaterialDTO>.Ok(createdMaterialDTO1));

        materialServiceMock
            .Setup(s => s.CreateMaterial(createMaterialDTO2))
            .ReturnsAsync(Response<MaterialDTO>.Ok(createdMaterialDTO2));

        // Act
        var result1 = await materialController.CreateMaterial(createMaterialDTO1);
        var result2 = await materialController.CreateMaterial(createMaterialDTO2);

        // Assert
        Assert.That(result1, Is.TypeOf<CreatedAtActionResult>());
        Assert.That(result2, Is.TypeOf<CreatedAtActionResult>());
        materialServiceMock.Verify(s => s.CreateMaterial(It.IsAny<CreateMaterialDTO>()), Times.Exactly(2));
    }

    #endregion

    #region UpdateMaterial Tests

    [Test]
    public async Task UpdateMaterial_WithValidData_ReturnsOkResponse()
    {
        // Arrange
        var materialId = 1;
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = materialId,
            Version = 1,
            Name = "Updated Material",
            Content = "Updated content"
        };

        var updatedMaterialDTO = new MaterialDTO
        {
            Id = materialId,
            Name = "Updated Material",
            Content = "Updated content",
            Version = 2
        };

        var response = Response<MaterialDTO>.Ok(updatedMaterialDTO);

        materialServiceMock
            .Setup(s => s.UpdateMaterial(updateMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.UpdateMaterial(1, updateMaterialDTO);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(updatedMaterialDTO));
        materialServiceMock.Verify(s => s.UpdateMaterial(updateMaterialDTO), Times.Once);
    }

    [Test]
    public async Task UpdateMaterial_WithNonExistentMaterial_ReturnsBadRequest()
    {
        // Arrange
        var materialId = 999;
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = materialId,
            Version = 1,
            Name = "Updated Material",
            Content = "Updated content"
        };

        var response = Response<MaterialDTO>.Fail("Material not found");

        materialServiceMock
            .Setup(s => s.UpdateMaterial(updateMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.UpdateMaterial(1, updateMaterialDTO);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        materialServiceMock.Verify(s => s.UpdateMaterial(updateMaterialDTO), Times.Once);
    }

    [Test]
    public async Task UpdateMaterial_WithIncorrectVersion_ReturnsBadRequest()
    {
        // Arrange
        var materialId = 1;
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = materialId,
            Version = 1,
            Name = "Updated Material",
            Content = "Updated content"
        };

        var response = Response<MaterialDTO>.Fail("Material not found");

        materialServiceMock
            .Setup(s => s.UpdateMaterial(updateMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.UpdateMaterial(1, updateMaterialDTO);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
    }

    [Test]
    public async Task UpdateMaterial_WithValidVersionIncrement_ReturnsUpdatedVersion()
    {
        // Arrange
        var materialId = 1;
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = materialId,
            Version = 5,
            Name = "Updated Material",
            Content = "Updated content"
        };

        var updatedMaterialDTO = new MaterialDTO
        {
            Id = materialId,
            Name = "Updated Material",
            Content = "Updated content",
            Version = 6
        };

        var response = Response<MaterialDTO>.Ok(updatedMaterialDTO);

        materialServiceMock
            .Setup(s => s.UpdateMaterial(updateMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.UpdateMaterial(1, updateMaterialDTO);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var materialDTO = okResult!.Value as MaterialDTO;
        Assert.That(materialDTO!.Version, Is.EqualTo(6));
    }

    [Test]
    public async Task UpdateMaterial_WithNameChange_ReturnsUpdatedName()
    {
        // Arrange
        var materialId = 1;
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = materialId,
            Version = 1,
            Name = "New Name",
            Content = "Same content"
        };

        var updatedMaterialDTO = new MaterialDTO
        {
            Id = materialId,
            Name = "New Name",
            Content = "Same content",
            Version = 2
        };

        var response = Response<MaterialDTO>.Ok(updatedMaterialDTO);

        materialServiceMock
            .Setup(s => s.UpdateMaterial(updateMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.UpdateMaterial(1, updateMaterialDTO);

        // Assert
        var okResult = result as OkObjectResult;
        var materialDTO = okResult!.Value as MaterialDTO;
        Assert.That(materialDTO!.Name, Is.EqualTo("New Name"));
    }

    [Test]
    public async Task UpdateMaterial_WithContentChange_ReturnsUpdatedContent()
    {
        // Arrange
        var materialId = 1;
        var updateMaterialDTO = new UpdateMaterialDTO
        {
            Id = materialId,
            Version = 1,
            Name = "Same name",
            Content = "New content"
        };

        var updatedMaterialDTO = new MaterialDTO
        {
            Id = materialId,
            Name = "Same name",
            Content = "New content",
            Version = 2
        };

        var response = Response<MaterialDTO>.Ok(updatedMaterialDTO);

        materialServiceMock
            .Setup(s => s.UpdateMaterial(updateMaterialDTO))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.UpdateMaterial(1, updateMaterialDTO);

        // Assert
        var okResult = result as OkObjectResult;
        var materialDTO = okResult!.Value as MaterialDTO;
        Assert.That(materialDTO!.Content, Is.EqualTo("New content"));
    }

    #endregion

    #region GetMaterialByLessonId Tests

    [Test]
    public async Task GetMaterialByLessonId_WithValidLessonId_ReturnsOkResponse()
    {
        // Arrange
        var lessonId = 1;
        var materialsDTO = new List<MaterialDTO>
        {
            new MaterialDTO { Id = 1, Name = "Material 1", Content = "Content 1", Version = 1 },
            new MaterialDTO { Id = 2, Name = "Material 2", Content = "Content 2", Version = 1 }
        };

        var response = Response<IList<MaterialDTO>>.Ok(materialsDTO);

        materialServiceMock
            .Setup(s => s.GetMaterialByLessonId(lessonId))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        var returnedMaterials = okResult.Value as IList<MaterialDTO>;
        Assert.That(returnedMaterials!.Count, Is.EqualTo(2));
        materialServiceMock.Verify(s => s.GetMaterialByLessonId(lessonId), Times.Once);
    }

    [Test]
    public async Task GetMaterialByLessonId_WithNoMaterials_ReturnsEmptyList()
    {
        // Arrange
        var lessonId = 999;
        var materialsDTO = new List<MaterialDTO>();

        var response = Response<IList<MaterialDTO>>.Ok(materialsDTO);

        materialServiceMock
            .Setup(s => s.GetMaterialByLessonId(lessonId))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var returnedMaterials = okResult!.Value as IList<MaterialDTO>;
        Assert.That(returnedMaterials!.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetMaterialByLessonId_WithMultipleMaterials_ReturnsAllMaterials()
    {
        // Arrange
        var lessonId = 1;
        var materialsDTO = new List<MaterialDTO>
        {
            new MaterialDTO { Id = 1, Name = "Material 1", Content = "Content 1", Version = 1 },
            new MaterialDTO { Id = 2, Name = "Material 2", Content = "Content 2", Version = 1 },
            new MaterialDTO { Id = 3, Name = "Material 3", Content = "Content 3", Version = 1 }
        };

        var response = Response<IList<MaterialDTO>>.Ok(materialsDTO);

        materialServiceMock
            .Setup(s => s.GetMaterialByLessonId(lessonId))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GetMaterialByLessonId(lessonId);

        // Assert
        var okResult = result as OkObjectResult;
        var returnedMaterials = okResult!.Value as IList<MaterialDTO>;
        Assert.That(returnedMaterials!.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetMaterialByLessonId_WithFailedResponse_ReturnsErrorStatus()
    {
        // Arrange
        var lessonId = 1;
        var response = Response<IList<MaterialDTO>>.Fail("Error retrieving materials");

        materialServiceMock
            .Setup(s => s.GetMaterialByLessonId(lessonId))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.GetMaterialByLessonId(lessonId);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        materialServiceMock.Verify(s => s.GetMaterialByLessonId(lessonId), Times.Once);
    }

    [Test]
    public async Task GetMaterialByLessonId_WithDifferentLessonIds_ReturnsCorrectMaterials()
    {
        // Arrange
        var lessonId1 = 1;
        var lessonId2 = 2;

        var materials1 = new List<MaterialDTO>
        {
            new MaterialDTO { Id = 1, Name = "Material 1", Content = "Content 1", Version = 1 }
        };

        var materials2 = new List<MaterialDTO>
        {
            new MaterialDTO { Id = 2, Name = "Material 2", Content = "Content 2", Version = 1 },
            new MaterialDTO { Id = 3, Name = "Material 3", Content = "Content 3", Version = 1 }
        };

        materialServiceMock
            .Setup(s => s.GetMaterialByLessonId(lessonId1))
            .ReturnsAsync(Response<IList<MaterialDTO>>.Ok(materials1));

        materialServiceMock
            .Setup(s => s.GetMaterialByLessonId(lessonId2))
            .ReturnsAsync(Response<IList<MaterialDTO>>.Ok(materials2));

        // Act
        var result1 = await materialController.GetMaterialByLessonId(lessonId1);
        var result2 = await materialController.GetMaterialByLessonId(lessonId2);

        // Assert
        var okResult1 = result1 as OkObjectResult;
        var okResult2 = result2 as OkObjectResult;
        var returnedMaterials1 = okResult1!.Value as IList<MaterialDTO>;
        var returnedMaterials2 = okResult2!.Value as IList<MaterialDTO>;
        Assert.That(returnedMaterials1!.Count, Is.EqualTo(1));
        Assert.That(returnedMaterials2!.Count, Is.EqualTo(2));
    }

    #endregion

    #region DeleteMaterial Tests

    [Test]
    public async Task DeleteMaterial_WithValidMaterialId_ReturnsOkResponse()
    {
        // Arrange
        var materialId = 1;
        var response = Response<bool>.Ok(true);

        materialServiceMock
            .Setup(s => s.DeleteMaterial(materialId))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.DeleteMaterial(materialId);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
        var okResult = result as NoContentResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(204));
        materialServiceMock.Verify(s => s.DeleteMaterial(materialId), Times.Once);
    }

    [Test]
    public async Task DeleteMaterial_WithNonExistentMaterialId_StillReturnsTrue()
    {
        // Arrange
        var materialId = 999;
        var response = Response<bool>.Ok(true);

        materialServiceMock
            .Setup(s => s.DeleteMaterial(materialId))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.DeleteMaterial(materialId);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task DeleteMaterial_WithFailedDeletion_ReturnsErrorStatus()
    {
        // Arrange
        var materialId = 1;
        var response = Response<bool>.Fail("Error deleting material");

        materialServiceMock
            .Setup(s => s.DeleteMaterial(materialId))
            .ReturnsAsync(response);

        // Act
        var result = await materialController.DeleteMaterial(materialId);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        materialServiceMock.Verify(s => s.DeleteMaterial(materialId), Times.Once);
    }

    [Test]
    public async Task DeleteMaterial_WithMultipleDeletions_DeletesEach()
    {
        // Arrange
        var materialIds = new[] { 1, 2, 3 };

        foreach (var id in materialIds)
        {
            materialServiceMock
                .Setup(s => s.DeleteMaterial(id))
                .ReturnsAsync(Response<bool>.Ok(true));
        }

        // Act & Assert
        foreach (var id in materialIds)
        {
            var result = await materialController.DeleteMaterial(id);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        materialServiceMock.Verify(s => s.DeleteMaterial(It.IsAny<int>()), Times.Exactly(3));
    }

    [Test]
    public async Task DeleteMaterial_CorrectlyPassesMaterialId()
    {
        // Arrange
        var materialId = 42;
        var response = Response<bool>.Ok(true);

        materialServiceMock
            .Setup(s => s.DeleteMaterial(materialId))
            .ReturnsAsync(response);

        // Act
        await materialController.DeleteMaterial(materialId);

        // Assert
        materialServiceMock.Verify(s => s.DeleteMaterial(42), Times.Once);
    }

    #endregion
}
