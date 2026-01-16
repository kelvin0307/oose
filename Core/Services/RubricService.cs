using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Extensions.ModelExtensions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class RubricService(
    IRubricRepository rubricRepository,
    IRepository<AssessmentDimension> assessmentDimensionRepository,
    IRepository<AssessmentDimensionScore> assessmentDimensionScoreRepository,
    IRepository<LearningOutcome> learningOutcomeRepository, 
    IMapper mapper
    ) : IRubricService
{
    public async Task<Response<List<RubricDto>>> GetAllRubrics()
    {
        try
        {
            var rubrics = await rubricRepository.GetAll();
            var rubricDtos = rubrics.Select((rubric)=> rubric.ToDto(mapper)).ToList();
            return Response<List<RubricDto>>.Ok(rubricDtos);
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<List<RubricDto>>.Fail("Invalid operation while getting the rubrics", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<List<RubricDto>>.Fail("An unexpected error occurred while fetching the rubrics");
        }
    }
    
    public async Task<Response<RubricDto>> GetRubricById(int id)
    {
        try
        {
            var rubric = await rubricRepository.GetAggregate(id);

            if (rubric == null) 
                return Response<RubricDto>.NotFound("Rubric not found");
            
            return Response<RubricDto>.Ok(rubric.ToDto(mapper));
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<RubricDto>.Fail("Invalid operation while getting rubric", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<RubricDto>.Fail("An unexpected error occurred while fetching the rubric");
        }
    }

    public async Task<Response<List<RubricDto>>> GetRubricsByLearningOutcomeId(int learningOutcomeId)
    {
        try
        {
            var rubrics = await rubricRepository.GetAggregatesByLearningOutcomeId(learningOutcomeId);
            var rubricDtos = rubrics.Select((rubric)=> rubric.ToDto(mapper)).ToList();
            return Response<List<RubricDto>>.Ok(rubricDtos);
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<List<RubricDto>>.Fail("Invalid operation while creating rubric", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<List<RubricDto>>.Fail("An unexpected error occurred while creating the rubric");
        }
    }
    
    public async Task<Response<RubricDto>> CreateRubric(CreateRubricDto createRubricDto)
    {
        try
        {
            var learningOutcome = await learningOutcomeRepository.Get(createRubricDto.LearningOutcomeId);
            if (learningOutcome == null)
                return Response<RubricDto>.NotFound("Learning outcome not found");

            var rubric = createRubricDto.ToModel(mapper);
            
            // check if there are atleast 1 assessment dimension
            if (rubric.AssessmentDimensions.Count == 0)
            {
                return Response<RubricDto>.Fail("Could not create Rubric. There must be at least one assessment dimension");
            }
            if (createRubricDto.AssessmentDimensions.Any(dimension => dimension.AssessmentDimensionScores.Count < 2))
            {
                return Response<RubricDto>.Fail("Could not create Rubric. Every assessment dimension requires at least 2 assessment dimension scores");
            }
            
            var createdRubric = await rubricRepository.CreateAndCommit(rubric);
            return Response<RubricDto>.Ok(createdRubric.ToDto(mapper));
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<RubricDto>.Fail("Invalid operation while creating rubric", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<RubricDto>.Fail("An unexpected error occurred while creating the rubric");
        }
    }

    public async Task<Response<RubricDto>> UpdateRubric(int id, UpdateRubricDto dto)
    {
        try
        {
            var rubric = await rubricRepository.GetAggregate(id);
            if (rubric == null)
                return Response<RubricDto>.NotFound("Could not update Rubric. Rubric not found");

            // update root
            rubric.Name = dto.Name;

            var existingDimensions = rubric.AssessmentDimensions
                .ToDictionary(d => d.Id);

            // check if there is atleast 1 assessmentDimension 
            if (existingDimensions.Count == 0)
            {
                return Response<RubricDto>.Fail("Could not update Rubric. There must be at least one assessment dimension");
            }

            var incomingDimensionIds = new HashSet<int>();

            foreach (var dimDto in dto.AssessmentDimensions)
            {
                // check if there are atleast 2 assessmentDimensionScore
                if (dimDto.AssessmentDimensionScores.Count < 2)
                {
                    return Response<RubricDto>.Fail("Could not update Rubric. There is at least 1 assessment dimension with less then 2 assessment dimension scores");
                }
                if (dimDto.Id != 0 &&
                    existingDimensions.TryGetValue(dimDto.Id, out var dimension))
                {
                    // update tracked dimension
                    incomingDimensionIds.Add(dimension.Id);

                    dimension.Name = dimDto.Name;
                    dimension.NameCriterium = dimDto.NameCriterium;
                    dimension.Wage = dimDto.Wage;
                    dimension.MinimumScore = dimDto.MinimumScore;

                    MergeScores(dimension, dimDto);
                }
                else
                {
                    // new dimension
                    var newDimension = new AssessmentDimension
                    {
                        Name = dimDto.Name,
                        NameCriterium = dimDto.NameCriterium,
                        Wage = dimDto.Wage,
                        MinimumScore = dimDto.MinimumScore
                    };

                    foreach (var scoreDto in dimDto.AssessmentDimensionScores)
                    {
                        newDimension.AssessmentDimensionScores.Add(
                            new AssessmentDimensionScore
                            {
                                Score = scoreDto.Score,
                                Description = scoreDto.Description
                            }
                        );
                    }

                    rubric.AssessmentDimensions.Add(newDimension);
                }
            }

            // remove deleted dimensions
            var dimensionsToRemove = rubric.AssessmentDimensions
                .Where(d => d.Id != 0 && !incomingDimensionIds.Contains(d.Id))
                .ToList();

            foreach (var dimension in dimensionsToRemove)
                rubric.AssessmentDimensions.Remove(dimension);

            await rubricRepository.SaveAggregate();

            return Response<RubricDto>.Ok(rubric.ToDto(mapper));
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<RubricDto>.Fail("Invalid operation while updating rubric", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<RubricDto>.Fail("An unexpected error occurred while updating the rubric");
        }
    }
    
    public async Task<Response<bool>> DeleteRubric(int id)
    {
        try
        {
            var course = await rubricRepository.Get(id);
            if (course == null)
                return Response<bool>.NotFound("Rubric not found");

            await rubricRepository.DeleteAndCommit(id);
            return Response<bool>.Ok(true);
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<bool>.Fail("Invalid operation while deleting rubric", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<bool>.Fail("An unexpected error occurred while deleting the rubric");
        }
    }
    
    private static void MergeScores(
        AssessmentDimension dimension,
        UpdateAssessmentDimensionDto dimDto)
    {
        var existingScores = dimension.AssessmentDimensionScores
            .ToDictionary(s => s.Id);

        var incomingScoreIds = new HashSet<int>();

        foreach (var scoreDto in dimDto.AssessmentDimensionScores)
        {
            if (scoreDto.Id != 0 &&
                existingScores.TryGetValue(scoreDto.Id, out var score))
            {
                // update tracked score
                incomingScoreIds.Add(score.Id);
                score.Score = scoreDto.Score;
                score.Description = scoreDto.Description;
            }
            else
            {
                // new score
                dimension.AssessmentDimensionScores.Add(
                    new AssessmentDimensionScore
                    {
                        Score = scoreDto.Score,
                        Description = scoreDto.Description
                    }
                );
            }
        }

        // remove deleted scores
        var scoresToRemove = dimension.AssessmentDimensionScores
            .Where(s => s.Id != 0 && !incomingScoreIds.Contains(s.Id))
            .ToList();

        foreach (var score in scoresToRemove)
            dimension.AssessmentDimensionScores.Remove(score);
    }
}