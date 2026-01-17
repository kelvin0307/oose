using AutoMapper;
using Core.Common;
using Core.DTOs;
using Data.Interfaces.Repositories;
using Core.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;

namespace Core.Tests.Services;

[TestFixture]
public class RubricServiceTests
{
    private Mock<IRubricRepository> rubricRepositoryMock;
    private Mock<IRepository<AssessmentDimension>> assessmentDimensionRepositoryMock;
    private Mock<IRepository<AssessmentDimensionScore>> assessmentDimensionScoreRepositoryMock;
    private Mock<IRepository<LearningOutcome>> learningOutcomeRepositoryMock;
    private Mock<IMapper> mapperMock;

    private RubricService rubricService;

    [SetUp]
    public void Setup()
    {
        rubricRepositoryMock = new Mock<IRubricRepository>();
        assessmentDimensionRepositoryMock = new Mock<IRepository<AssessmentDimension>>();
        assessmentDimensionScoreRepositoryMock = new Mock<IRepository<AssessmentDimensionScore>>();
        learningOutcomeRepositoryMock = new Mock<IRepository<LearningOutcome>>();
        mapperMock = new Mock<IMapper>();

        rubricService = new RubricService(
            rubricRepositoryMock.Object,
            assessmentDimensionRepositoryMock.Object,
            assessmentDimensionScoreRepositoryMock.Object,
            learningOutcomeRepositoryMock.Object,
            mapperMock.Object
        );
    }

    #region GetRubricById

    [Test]
    public async Task GetRubricById_WithExistingRubric_ReturnsOk()
    {
        // Arrange
        var rubric = new Rubric { Id = 1, Name = "Test Rubric" };
        var rubricDto = new RubricDto { Id = 1, Name = "Test Rubric" };

        rubricRepositoryMock
            .Setup(r => r.GetAggregate(1))
            .ReturnsAsync(rubric);

        mapperMock
            .Setup(m => m.Map<RubricDto>(rubric))
            .Returns(rubricDto);

        // Act
        var result = await rubricService.GetRubricById(1);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Id, Is.EqualTo(1));
        Assert.That(result.Result.Name, Is.EqualTo("Test Rubric"));

        rubricRepositoryMock.Verify(r => r.GetAggregate(1), Times.Once);
        mapperMock.Verify(m => m.Map<RubricDto>(rubric), Times.Once);
    }

    [Test]
    public async Task GetRubricById_WhenRubricNotFound_ReturnsNotFound()
    {
        rubricRepositoryMock
            .Setup(r => r.GetAggregate(1))
            .ReturnsAsync((Rubric)null);

        var result = await rubricService.GetRubricById(1);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Rubric not found"));
    }

    #endregion

    #region CreateRubric

    [Test]
    public async Task CreateRubric_WithValidInput_ReturnsOk()
    {
        // Arrange
        var dto = new CreateRubricDto
        {
            Name = "Test Rubric",
            LearningOutcomeId = 1,
            AssessmentDimensions =
            {
                new CreateAssessmentDimensionDto
                {
                    Name = "Dimension 1",
                    AssessmentDimensionScores =
                    {
                        new CreateAssessmentDimensionScoreDto { Score = 1, Description = "Low" },
                        new CreateAssessmentDimensionScoreDto { Score = 2, Description = "High" }
                    }
                }
            }
        };

        var learningOutcome = new LearningOutcome { Id = 1 };

        var rubric = new Rubric
        {
            Name = "Test Rubric",
            AssessmentDimensions =
            {
                new AssessmentDimension
                {
                    AssessmentDimensionScores =
                    {
                        new AssessmentDimensionScore(),
                        new AssessmentDimensionScore()
                    }
                }
            }
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(dto.LearningOutcomeId))
            .ReturnsAsync(learningOutcome);

        mapperMock
            .Setup(m => m.Map<Rubric>(dto))
            .Returns(rubric);

        rubricRepositoryMock
            .Setup(r => r.CreateAndCommit(rubric))
            .ReturnsAsync(rubric);

        mapperMock
            .Setup(m => m.Map<RubricDto>(rubric))
            .Returns(new RubricDto { Name = "Test Rubric" });

        // Act
        var result = await rubricService.CreateRubric(dto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Name, Is.EqualTo("Test Rubric"));
        rubricRepositoryMock.Verify(r => r.CreateAndCommit(rubric), Times.Once);
    }

    [Test]
    public async Task CreateRubric_WhenLearningOutcomeNotFound_ReturnsNotFound()
    {
        var dto = new CreateRubricDto { LearningOutcomeId = 99 };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(dto.LearningOutcomeId))
            .ReturnsAsync((LearningOutcome)null);

        var result = await rubricService.CreateRubric(dto);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Learning outcome not found"));
    }

    [Test]
    public async Task CreateRubric_WithNoAssessmentDimensions_ReturnsFail()
    {
        var dto = new CreateRubricDto
        {
            LearningOutcomeId = 1,
            AssessmentDimensions = new List<CreateAssessmentDimensionDto>()
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(dto.LearningOutcomeId))
            .ReturnsAsync(new LearningOutcome { Id = 1 });

        mapperMock
            .Setup(m => m.Map<Rubric>(dto))
            .Returns(new Rubric { AssessmentDimensions = new List<AssessmentDimension>() });

        var result = await rubricService.CreateRubric(dto);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("at least one assessment dimension"));
    }

    [Test]
    public async Task CreateRubric_WithDimensionHavingLessThanTwoScores_ReturnsFail()
    {
        var dto = new CreateRubricDto
        {
            LearningOutcomeId = 1,
            AssessmentDimensions =
            {
                new CreateAssessmentDimensionDto
                {
                    AssessmentDimensionScores =
                    {
                        new CreateAssessmentDimensionScoreDto { Score = 1 }
                    }
                }
            }
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(dto.LearningOutcomeId))
            .ReturnsAsync(new LearningOutcome { Id = 1 });

        mapperMock
            .Setup(m => m.Map<Rubric>(dto))
            .Returns(new Rubric
            {
                AssessmentDimensions =
                {
                    new AssessmentDimension
                    {
                        AssessmentDimensionScores =
                        {
                            new AssessmentDimensionScore()
                        }
                    }
                }
            });

        var result = await rubricService.CreateRubric(dto);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("at least 2 assessment dimension scores"));
    }

    #endregion

    #region UpdateRubric

    [Test]
    public async Task UpdateRubric_WithValidInput_UpdatesAndSaves()
    {
        // Arrange
        var rubric = new Rubric
        {
            Id = 1,
            Name = "Old Name",
            AssessmentDimensions =
            {
                new AssessmentDimension
                {
                    Id = 1,
                    AssessmentDimensionScores =
                    {
                        new AssessmentDimensionScore { Id = 1, Score = 1 },
                        new AssessmentDimensionScore { Id = 2, Score = 2 }
                    }
                }
            }
        };

        var dto = new UpdateRubricDto
        {
            Name = "New Name",
            AssessmentDimensions =
            {
                new UpdateAssessmentDimensionDto
                {
                    Id = 1,
                    AssessmentDimensionScores =
                    {
                        new UpdateAssessmentDimensionScoreDto { Id = 1, Score = 10 },
                        new UpdateAssessmentDimensionScoreDto { Id = 2, Score = 20 }
                    }
                }
            }
        };

        rubricRepositoryMock
            .Setup(r => r.GetAggregate(1))
            .ReturnsAsync(rubric);

        rubricRepositoryMock
            .Setup(r => r.SaveAggregate())
            .Returns(Task.CompletedTask);

        mapperMock
            .Setup(m => m.Map<RubricDto>(rubric))
            .Returns(new RubricDto { Name = "New Name" });

        // Act
        var result = await rubricService.UpdateRubric(1, dto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(rubric.Name, Is.EqualTo("New Name"));
        rubricRepositoryMock.Verify(r => r.SaveAggregate(), Times.Once);
    }

    [Test]
    public async Task UpdateRubric_WhenRubricNotFound_ReturnsNotFound()
    {
        rubricRepositoryMock
            .Setup(r => r.GetAggregate(1))
            .ReturnsAsync((Rubric)null);

        var result = await rubricService.UpdateRubric(1, new UpdateRubricDto());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Rubric not found"));
    }

    [Test]
    public async Task UpdateRubric_WhenNoExistingDimensions_ReturnsFail()
    {
        var rubric = new Rubric
        {
            Id = 1,
            AssessmentDimensions = new List<AssessmentDimension>()
        };

        rubricRepositoryMock
            .Setup(r => r.GetAggregate(1))
            .ReturnsAsync(rubric);

        var result = await rubricService.UpdateRubric(1, new UpdateRubricDto());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("at least one assessment dimension"));
    }

    [Test]
    public async Task UpdateRubric_WhenDimensionHasLessThanTwoScores_ReturnsFail()
    {
        var rubric = new Rubric
        {
            Id = 1,
            AssessmentDimensions =
            {
                new AssessmentDimension { Id = 1 }
            }
        };

        var dto = new UpdateRubricDto
        {
            AssessmentDimensions =
            {
                new UpdateAssessmentDimensionDto
                {
                    Id = 1,
                    AssessmentDimensionScores =
                    {
                        new UpdateAssessmentDimensionScoreDto { Score = 1 }
                    }
                }
            }
        };

        rubricRepositoryMock
            .Setup(r => r.GetAggregate(1))
            .ReturnsAsync(rubric);

        var result = await rubricService.UpdateRubric(1, dto);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("less then 2 assessment dimension scores"));
    }

    #endregion

    #region DeleteRubric

    [Test]
    public async Task DeleteRubric_WhenExists_ReturnsOk()
    {
        var rubric = new Rubric { Id = 1 };

        rubricRepositoryMock
            .Setup(r => r.Get(1))
            .ReturnsAsync(rubric);

        rubricRepositoryMock
            .Setup(r => r.DeleteAndCommit(1))
            .ReturnsAsync(rubric);

        var result = await rubricService.DeleteRubric(1);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.True);
    }

    [Test]
    public async Task DeleteRubric_WhenNotFound_ReturnsNotFound()
    {
        rubricRepositoryMock
            .Setup(r => r.Get(1))
            .ReturnsAsync((Rubric)null);

        var result = await rubricService.DeleteRubric(1);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Rubric not found"));
    }

    #endregion
}