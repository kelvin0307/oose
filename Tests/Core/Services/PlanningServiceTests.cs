using AutoMapper;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Data.Interfaces.Repositories;
using Core.Services;
using Domain.Enums;
using Domain.Models;
using Moq;
using NUnit.Framework;

namespace Tests.Core.Services;

[TestFixture]
public class PlanningServiceTests
{
    private Mock<IRepository<Planning>> planningRepositoryMock;
    private Mock<IDocumentFactory> documentFactoryMock;
    private Mock<IMapper> mapperMock;
    private PlanningService planningService;

    [SetUp]
    public void Setup()
    {
        planningRepositoryMock = new Mock<IRepository<Planning>>();
        documentFactoryMock = new Mock<IDocumentFactory>();
        mapperMock = new Mock<IMapper>();

        planningService = new PlanningService(planningRepositoryMock.Object, documentFactoryMock.Object, mapperMock.Object);
    }

    #region GetPlanningByCourseId Tests

    [Test]
    public void GetPlanningByCourseId_WithValidCourseId_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var lessons = new List<Lesson>
        {
            new Lesson { Id = 1, SequenceNumber = 1, WeekNumber = 1, Name = "Lesson 1" },
            new Lesson { Id = 2, SequenceNumber = 2, WeekNumber = 1, Name = "Lesson 2" }
        };


        var lessonsDTOs = new List<LessonDTO>
        {
            new LessonDTO { Id = 1, SequenceNumber = 1, WeekNumber = 1, Name = "Lesson 1" },
            new LessonDTO { Id = 2, SequenceNumber = 2, WeekNumber = 1, Name = "Lesson 2" }
        };

        var planning = new Planning
        {
            Id = 1,
            CourseId = courseId,
            Lessons = lessons
        };

        var planningDto = new PlanningDTO
        {
            Id = 1,
            Lessons = lessonsDTOs
        };

        var queryablePlanning = new List<Planning> { planning }.AsQueryable();

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Returns(queryablePlanning);

        mapperMock.Setup(x => x.Map<PlanningDTO>(planning)).Returns(planningDto);

        // Act
        var result = planningService.GetPlanningByCourseId(courseId);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        planningRepositoryMock.Verify(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()), Times.Once);
    }

    [Test]
    public void GetPlanningByCourseId_WithNonExistentCourseId_ReturnsNotFound()
    {
        // Arrange
        var courseId = 999;
        var emptyPlanning = new List<Planning>().AsQueryable();

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Returns(emptyPlanning);

        // Act
        var result = planningService.GetPlanningByCourseId(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("planning not found"));
    }

    [Test]
    public void GetPlanningByCourseId_WhenInvalidOperationExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;
        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Throws(new InvalidOperationException("Invalid operation"));

        // Act
        var result = planningService.GetPlanningByCourseId(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid operation while getting course"));
        planningRepositoryMock.Verify(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()), Times.Once);
    }

    [Test]
    public void GetPlanningByCourseId_WhenGeneralExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;

        var lessons = new List<Lesson>
        {
            new Lesson { Id = 1, SequenceNumber = 1, WeekNumber = 1, Name = "Lesson 1" },
            new Lesson { Id = 2, SequenceNumber = 2, WeekNumber = 1, Name = "Lesson 2" }
        };
        var planning = new Planning
        {
            Id = 1,
            CourseId = courseId,
            Lessons = lessons
        };
        var queryablePlanning = new List<Planning> { planning }.AsQueryable();


        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Returns(queryablePlanning);

        mapperMock.Setup(m => m.Map<PlanningDTO>(It.IsAny<Planning>()))
            .Throws(new Exception("Mock Error was thrown"));

        // Act
        var result = planningService.GetPlanningByCourseId(courseId);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while fetching the course"));
        planningRepositoryMock.Verify(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()), Times.Once);
    }

    #endregion

    #region GenerateDocument Tests

    [Test]
    public async Task GenerateDocument_WithValidCourseIdAndDocumentType_ReturnsOkResponse()
    {
        // Arrange
        var courseId = 1;
        var documentType = DocumentTypes.Pdf;

        var lessons = new List<Lesson>
        {
            new Lesson { Id = 1, SequenceNumber = 1, WeekNumber = 1, Name = "Lesson 1" }
        };

        var planning = new Planning
        {
            Id = 1,
            CourseId = courseId,
            Lessons = lessons
        };

        var queryablePlanning = new List<Planning> { planning }.AsQueryable();
        var documentBytes = new byte[] { 1, 2, 3, 4, 5 };

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Returns(queryablePlanning.Where(x => x.CourseId == courseId));

        documentFactoryMock
            .Setup(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), documentType))
            .Returns(new DocumentDTO
            {
                Document = documentBytes,
                ContentType = "application/pdf",
                DocumentName = "planning.pdf"
            });

        // Act
        var result = await planningService.GenerateDocument(courseId, documentType);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Document, Is.EqualTo(documentBytes));
        Assert.That(result.Result.ContentType, Is.EqualTo("application/pdf"));
    }

    [Test]
    public async Task GenerateDocument_WithNonExistentCourseId_ReturnsFail()
    {
        // Arrange
        var courseId = 999;
        var documentType = DocumentTypes.Pdf;
        var emptyPlanning = new List<Planning>().AsQueryable();

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Returns(emptyPlanning);

        // Act
        var result = await planningService.GenerateDocument(courseId, documentType);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error generating planning document"));
    }

    [Test]
    public async Task GenerateDocument_WhenExceptionThrown_ReturnsFail()
    {
        // Arrange
        var courseId = 1;
        var documentType = DocumentTypes.Pdf;

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = await planningService.GenerateDocument(courseId, documentType);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Error generating planning document"));
    }

    [Test]
    public async Task GenerateDocument_WithDifferentDocumentTypes_CreatesCorrectDocument()
    {
        // Arrange
        var courseId = 1;
        var documentTypes = new[] { DocumentTypes.Pdf, DocumentTypes.Csv, DocumentTypes.Docx };

        var lessons = new List<Lesson>
        {
            new Lesson { Id = 1, SequenceNumber = 1, WeekNumber = 1, Name = "Lesson 1" }
        };

        var planning = new Planning
        {
            Id = 1,
            CourseId = courseId,
            Lessons = lessons
        };

        var queryablePlanning = new List<Planning> { planning }.AsQueryable();

        planningRepositoryMock
            .Setup(r => r.Include(It.IsAny<System.Linq.Expressions.Expression<System.Func<Planning, List<Lesson>>>>()))
            .Returns(queryablePlanning);

        documentFactoryMock
            .Setup(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), It.IsAny<DocumentTypes>()))
            .Returns((DocumentDataDTO data, DocumentTypes type) => new DocumentDTO
            {
                Document = new byte[] { 1, 2, 3 },
                ContentType = "application/octet-stream",
                DocumentName = $"planning.{type.ToString().ToLower()}"
            });

        // Act & Assert
        foreach (var docType in documentTypes)
        {
            var result = await planningService.GenerateDocument(courseId, docType);
            Assert.That(result.Success, Is.True);
        }

        documentFactoryMock.Verify(f => f.GenerateDocument(It.IsAny<DocumentDataDTO>(), It.IsAny<DocumentTypes>()), Times.AtLeastOnce);
    }

    #endregion
}
