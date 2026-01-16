using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
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
    public async Task CreateRubric_WithValidLearningOutcome_ReturnsOk()
    {
        // Arrange
        var dto = new CreateRubricDto
        {
            Name = "New Rubric",
            LearningOutcomeId = 10
        };

        var learningOutcome = new LearningOutcome { Id = 10 };
        var rubric = new Rubric { Id = 1, Name = "New Rubric" };
        var rubricDto = new RubricDto { Id = 1, Name = "New Rubric" };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(dto.LearningOutcomeId))
            .ReturnsAsync(learningOutcome);

        mapperMock.Setup(m => m.Map<Rubric>(dto)).Returns(rubric);

        rubricRepositoryMock
            .Setup(r => r.CreateAndCommit(rubric))
            .ReturnsAsync(rubric);

        mapperMock.Setup(m => m.Map<RubricDto>(rubric)).Returns(rubricDto);

        // Act
        var result = await rubricService.CreateRubric(dto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result.Name, Is.EqualTo("New Rubric"));

        rubricRepositoryMock.Verify(r => r.CreateAndCommit(rubric), Times.Once);
    }

    [Test]
    public async Task CreateRubric_WhenLearningOutcomeNotFound_ReturnsNotFound()
    {
        var dto = new CreateRubricDto
        {
            Name = "New Rubric",
            LearningOutcomeId = 99
        };

        learningOutcomeRepositoryMock
            .Setup(r => r.Get(dto.LearningOutcomeId))
            .ReturnsAsync((LearningOutcome)null);

        var result = await rubricService.CreateRubric(dto);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Learning outcome not found"));
    }

    #endregion

    #region UpdateRubric

    [Test]
    public async Task UpdateRubric_UpdatesExistingDimensionAndScore()
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
                    Name = "Old Dimension",
                    AssessmentDimensionScores =
                    {
                        new AssessmentDimensionScore
                        {
                            Id = 1,
                            Score = 5,
                            Description = "Old Score"
                        }
                    }
                }
            }
        };

        var updateDto = new UpdateRubricDto
        {
            Name = "New Name",
            AssessmentDimensions =
            {
                new UpdateAssessmentDimensionDto
                {
                    Id = 1,
                    Name = "Updated Dimension",
                    AssessmentDimensionScores =
                    {
                        new UpdateAssessmentDimensionScoreDto
                        {
                            Id = 1,
                            Score = 10,
                            Description = "Updated Score"
                        }
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
            .Returns(new RubricDto { Id = 1, Name = "New Name" });

        // Act
        var result = await rubricService.UpdateRubric(1, updateDto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(rubric.Name, Is.EqualTo("New Name"));

        var dimension = rubric.AssessmentDimensions.Single();
        Assert.That(dimension.Name, Is.EqualTo("Updated Dimension"));

        var score = dimension.AssessmentDimensionScores.Single();
        Assert.That(score.Score, Is.EqualTo(10));
        Assert.That(score.Description, Is.EqualTo("Updated Score"));

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