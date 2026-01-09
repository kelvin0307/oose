using Core.Common;
using Core.DTOs;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Mappers;
using Domain.Models;

namespace Core.Services;

public class LearningOutcomeService(
    IRepository<LearningOutcome> learningOutcomeRepository, 
    IRepository<Course> courseRepository)
    : ILearningOutcomeService
{
    public async Task<Response<List<LearningOutcomeDto>>> GetAllLearningOutcomes()
    {
        try
        {
            var learningOutcomes = await learningOutcomeRepository.GetAll();
            return Response<List<LearningOutcomeDto>>.Ok(learningOutcomes.Select(LearningOutcomeMapper.ToDto).ToList());
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<List<LearningOutcomeDto>>.Fail("Invalid operation while fetching learning outcomes", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<List<LearningOutcomeDto>>.Fail("An unexpected error occurred while fetching learning outcomes");
        }
    }
    
    public async Task<Response<List<LearningOutcomeDto>>> GetAllLearningOutcomesByCourseId(int courseId)
    {
        try
        {
            var course = await courseRepository.Get(courseId);
            if (course == null) 
                return Response<List<LearningOutcomeDto>>.NotFound("Course not found");
            
            var learningOutcomes = learningOutcomeRepository.GetLearningOutcomesByCourseId(courseId).ToList();
            return Response<List<LearningOutcomeDto>>.Ok(learningOutcomes.Select(LearningOutcomeMapper.ToDto).ToList());
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<List<LearningOutcomeDto>>.Fail("Invalid operation while fetching learning outcomes", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<List<LearningOutcomeDto>>.Fail("An unexpected error occurred while fetching learning outcomes");
        }
    }
    
    public async Task<Response<LearningOutcomeDto>> GetLearningOutcomeById(int id)
    {
        try
        {
            var learningOutcome = await learningOutcomeRepository.Get(id);
    
            return learningOutcome != null 
                ? Response<LearningOutcomeDto>.Ok(LearningOutcomeMapper.ToDto(learningOutcome))
                : Response<LearningOutcomeDto>.NotFound("Learning outcome not found");
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<LearningOutcomeDto>.Fail("Invalid operation while getting learning outcome", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<LearningOutcomeDto>.Fail("An unexpected error occurred while fetching the learning outcome");
        }
    }
    
    public async Task<Response<LearningOutcomeDto>> CreateLearningOutcome(CreateLearningOutcomeDto createLearningOutcomeDto)
    {
        try
        {
            var course = await courseRepository.Get(createLearningOutcomeDto.CourseId);
            if (course == null) 
                return Response<LearningOutcomeDto>.NotFound("Course not found");
            
            var learningOutcome = new LearningOutcome
            {
                Name =  createLearningOutcomeDto.Name,
                Description = createLearningOutcomeDto.Description,
                EndQualification = createLearningOutcomeDto.EndQualification,
                CourseId = createLearningOutcomeDto.CourseId
            };

    
            var createdLearningOutcome = await learningOutcomeRepository.CreateAndCommit(learningOutcome);
            return Response<LearningOutcomeDto>.Ok(LearningOutcomeMapper.ToDto(createdLearningOutcome));
        }   
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<LearningOutcomeDto>.Fail("Invalid operation while creating learning outcome", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<LearningOutcomeDto>.Fail("An unexpected error occurred while creating the learning outcome");
        }
    }
    
    public async Task<Response<LearningOutcomeDto>> UpdateLearningOutcome(int id, UpdateLearningOutcomeDto learningOutcomeDto)
    {
        try
        {
            var learningOutcome = await learningOutcomeRepository.Get(id);
            if (learningOutcome == null)
                return Response<LearningOutcomeDto>.NotFound("Learning outcome not found");
    
            learningOutcome.Name = learningOutcomeDto.Name;
            learningOutcome.Description = learningOutcomeDto.Description;
            learningOutcome.EndQualification = learningOutcomeDto.EndQualification;
            
    
            var updatedLearningOutcome = await learningOutcomeRepository.UpdateAndCommit(learningOutcome);
            return Response<LearningOutcomeDto>.Ok(LearningOutcomeMapper.ToDto(updatedLearningOutcome));
        }  
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<LearningOutcomeDto>.Fail("Invalid operation while updating learning outcome", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<LearningOutcomeDto>.Fail("An unexpected error occurred while updating the learning outcome");
        }
    }
    
    public async Task<Response<bool>> DeleteLearningOutcome(int id)
    {
        try
        {
            var learningOutcome = await learningOutcomeRepository.Get(id);
            if (learningOutcome == null)
                return Response<bool>.NotFound("Learning outcome not found");
            
            await learningOutcomeRepository.DeleteAndCommit(id);
            return Response<bool>.Ok(true);
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<bool>.Fail("Invalid operation while deleting learning outcome", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<bool>.Fail("An unexpected error occurred while deleting the learning outcome");
        }
    }
}