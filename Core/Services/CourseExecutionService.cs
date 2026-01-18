using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Extensions.ModelExtensions;
using Core.Interfaces.Services;
using Core.Interfaces.Repositories;
using Domain.Models;

namespace Core.Services;

public class CourseExecutionService(IRepository<CourseExecution> courseExecutionRepository, IRepository<Course> courseRepository, IMapper mapper): ICourseExecutionService
{
    public async Task<Response<List<CourseExecutionDTO>>> GetAllCourseExecutions()
    {
        var response = await courseExecutionRepository.GetAll();
        return Response<List<CourseExecutionDTO>>.Ok(response.Select(execution => execution.ToDto(mapper)).ToList());
    }
    
    public async Task<Response<CourseExecutionDTO>> GetCourseExecutionById(int id)
    {
        try
        {
            var courseExecution = courseExecutionRepository.Include(course => course.Course).FirstOrDefault(execution => execution.Id == id);
            if (courseExecution == null)
            {
                return Response<CourseExecutionDTO>.NotFound("Course execution not found");
            }
            
            return Response<CourseExecutionDTO>.Ok(courseExecution.ToDto(mapper));
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDTO>.Fail("Invalid operation while creating the course execution", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDTO>.Fail("An unexpected error occurred while creating a course execution");
        }    }

    public async Task<Response<CourseExecutionDTO>> CreateCourseExecution(CreateCourseExecutionDto createCourseExecutionDto)
    {
        try
        {
            if (createCourseExecutionDto.StartDate >= createCourseExecutionDto.EndDate)
            {
                return Response<CourseExecutionDTO>.Fail(
                    "StartDate must be before EndDate",
                    ResponseStatus.ValidationError);
            }
            var course = await courseRepository.Get(createCourseExecutionDto.CourseId);
            if (course == null)
            {
                return Response<CourseExecutionDTO>.NotFound("Course not found");
            }
            
            var execution = new CourseExecution
            {
                Course = course,
                StartDate = createCourseExecutionDto.StartDate,
                EndDate = createCourseExecutionDto.EndDate,
                Classes = new List<Class>()
            };

            var savedExecution = await courseExecutionRepository.CreateAndCommit(execution);
            
            return Response<CourseExecutionDTO>.Ok(savedExecution.ToDto(mapper));
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDTO>.Fail("Invalid operation while creating the course execution", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDTO>.Fail("An unexpected error occurred while creating a course execution");
        }
    }
    
    public async Task<Response<CourseExecutionDTO>> EndCourseExecution(int id)
    {
        try
        {
            var execution = await courseExecutionRepository.Get(id);
            if (execution == null)
            {
                return Response<CourseExecutionDTO>.NotFound("Course execution not found");
            }

            if (execution.EndDate <= DateTime.UtcNow)
            {
                return Response<CourseExecutionDTO>.Fail(
                    "Course execution has already ended");
            }

            execution.EndDate = DateTime.UtcNow;

            var updatedExecution =
                await courseExecutionRepository.UpdateAndCommit(execution);

            return Response<CourseExecutionDTO>.Ok(updatedExecution.ToDto(mapper));
        }
        catch (InvalidOperationException)
        {
            return Response<CourseExecutionDTO>.Fail(
                "Invalid operation while ending the course execution",
                ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<CourseExecutionDTO>.Fail(
                "An unexpected error occurred while ending the course execution");
        }
    }
}