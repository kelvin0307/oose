using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class CourseService(IRepository<Course> courseRepository) : ICourseService
{
    public async Task<Response<List<CourseDto>>> GetAllCourses()
    {
        try
        {
            var courses = await courseRepository.GetAll();
            return Response<List<CourseDto>>.Ok(courses.Select(MapToDto).ToList());
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<List<CourseDto>>.Fail("Invalid operation while fetching course", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<List<CourseDto>>.Fail("An unexpected error occurred while fetching courses");
        }
        
    }
    
    public async Task<Response<CourseDto>> GetCourseById(int id)
    {
        try
        {
            var course = await courseRepository.Get(id);

            return course != null 
                ? Response<CourseDto>.Ok(MapToDto(course))
                : Response<CourseDto>.NotFound("Course not found");
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("Invalid operation while getting course", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("An unexpected error occurred while fetching the course");
        }
        
    }
    
    public async Task<Response<CourseDto>> CreateCourse(CreateCourseDto createCourseDto)
    {
        try
        {
            var course = new Course
            {
                Name = createCourseDto.Name,
                Description = createCourseDto.Description,
                Status = CourseStatus.Concept
            };

            var createdCourse = await courseRepository.CreateAndCommit(course);
            return Response<CourseDto>.Ok(MapToDto(createdCourse));
        }   
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("Invalid operation while updating course", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("An unexpected error occurred while updating the course");
        }
        
    }
    
    public async Task<Response<CourseDto>> UpdateCourse(int id, UpdateCourseDto updateCourseDto)
    {
        try
        {
            var course = await courseRepository.Get(id);
            if (course == null)
                return Response<CourseDto>.NotFound("Course not found");    

            course.Name = updateCourseDto.Name;
            course.Description = updateCourseDto.Description;

            var updatedCourse = await courseRepository.UpdateAndCommit(course);
            return Response<CourseDto>.Ok(MapToDto(updatedCourse));
        }  
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("Invalid operation while updating course", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("An unexpected error occurred while updating the course");
        }
        
    }
    
    public async Task<Response<bool>> DeleteCourse(int id)
    {
        try
        {
            var course = await courseRepository.Get(id);
            if (course == null)
                return Response<bool>.NotFound("Course not found");
            
            await courseRepository.DeleteAndCommit(id);
            return Response<bool>.Ok(true);
        }
        catch (InvalidOperationException ex)
        {
            //TODO: Log exception
            return Response<bool>.Fail("Invalid operation while deleting course", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<bool>.Fail("An unexpected error occurred while deleting the course");
        }
    }


    
    private CourseDto MapToDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
        };
    }
}