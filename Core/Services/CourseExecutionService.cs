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
    public async Task<Response<List<CourseExecutionDto>>> GetAllCourseExecutions()
    {
        var response = await courseExecutionRepository.GetAll();
        return Response<List<CourseExecutionDto>>.Ok(response.Select(execution => execution.ToDto(mapper)).ToList());
    }
    
    public async Task<Response<CourseExecutionDto>> GetCourseExecutionById(int id)
    {
        try
        {
            var courseExecution = courseExecutionRepository.Include(course => course.Course).FirstOrDefault(execution => execution.Id == id);
            if (courseExecution == null)
            {
                return Response<CourseExecutionDto>.NotFound("Course execution not found");
            }
            
            return Response<CourseExecutionDto>.Ok(courseExecution.ToDto(mapper));
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDto>.Fail("Invalid operation while creating the course execution", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDto>.Fail("An unexpected error occurred while creating a course execution");
        }    }

    public async Task<Response<CourseExecutionDto>> CreateCourseExecution(CreateCourseExecutionDto createCourseExecutionDto)
    {
        try
        {
            if (createCourseExecutionDto.StartDate >= createCourseExecutionDto.EndDate)
            {
                return Response<CourseExecutionDto>.Fail(
                    "StartDate must be before EndDate",
                    ResponseStatus.ValidationError);
            }
            var course = await courseRepository.Get(createCourseExecutionDto.CourseId);
            if (course == null)
            {
                return Response<CourseExecutionDto>.NotFound("Course not found");
            }
            
            var execution = new CourseExecution
            {
                Course = course,
                StartDate = createCourseExecutionDto.StartDate,
                EndDate = createCourseExecutionDto.EndDate,
                Classes = new List<Class>()
            };

            var savedExecution = await courseExecutionRepository.CreateAndCommit(execution);
            
            return Response<CourseExecutionDto>.Ok(savedExecution.ToDto(mapper));
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDto>.Fail("Invalid operation while creating the course execution", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseExecutionDto>.Fail("An unexpected error occurred while creating a course execution");
        }
    }
    
    public async Task<Response<CourseExecutionDto>> EndCourseExecution(int id)
    {
        try
        {
            var execution = await courseExecutionRepository.Get(id);
            if (execution == null)
            {
                return Response<CourseExecutionDto>.NotFound("Course execution not found");
            }

            if (execution.EndDate <= DateTime.UtcNow)
            {
                return Response<CourseExecutionDto>.Fail(
                    "Course execution has already ended");
            }

            execution.EndDate = DateTime.UtcNow;

            var updatedExecution =
                await courseExecutionRepository.UpdateAndCommit(execution);

            return Response<CourseExecutionDto>.Ok(updatedExecution.ToDto(mapper));
        }
        catch (InvalidOperationException)
        {
            return Response<CourseExecutionDto>.Fail(
                "Invalid operation while ending the course execution",
                ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<CourseExecutionDto>.Fail(
                "An unexpected error occurred while ending the course execution");
        }
    }
}