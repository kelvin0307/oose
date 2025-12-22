using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class CourseService(IRepository<Course> courseRepository) : ICourseService
{
    public async Task<Response<CourseDto>> CreateCourse(CreateCourseDto createCourseDto)
    {
        try
        {
            var course = new Course
            {
                Name = createCourseDto.Name,
                Description = createCourseDto.Description,
            };

            var createdCourse = await courseRepository.CreateAndCommit(course);
            return Response<CourseDto>.Ok(MapToDto(createdCourse));
        }   
        catch (InvalidOperationException ex)
        {
            return Response<CourseDto>.Fail("Invalid operation while updating course", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            return Response<CourseDto>.Fail("An unexpected error occurred while updating the course");
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