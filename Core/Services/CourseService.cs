using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Extensions.ModelExtensions;
using Data.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class CourseService(IRepository<Course> courseRepository, IMapper mapper) : ICourseService
{
    public async Task<Response<List<CourseDTO>>> GetAllCourses()
    {
        try
        {
            var courses = await courseRepository.GetAll();
            return Response<List<CourseDTO>>.Ok(courses.Select(x => x.ToDto(mapper)).ToList());
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<List<CourseDTO>>.Fail("Invalid operation while fetching course", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            //TODO: Log exception
            return Response<List<CourseDTO>>.Fail("An unexpected error occurred while fetching courses");
        }

    }

    public async Task<Response<CourseDTO>> GetCourseById(int id)
    {
        try
        {
            var course = await courseRepository.Get(id);

            return course != null
                ? Response<CourseDTO>.Ok(course.ToDto(mapper))
                : Response<CourseDTO>.NotFound("Course not found");
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("Invalid operation while getting course", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("An unexpected error occurred while fetching the course");
        }

    }

    public async Task<Response<CourseDTO>> CreateCourse(CreateCourseDto createCourseDto)
    {
        try
        {
            var course = createCourseDto.ToModel(mapper);
            course.Status = CourseStatus.Concept;

            var createdCourse = await courseRepository.CreateAndCommit(course);
            return Response<CourseDTO>.Ok(createdCourse.ToDto(mapper));
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("Invalid operation while updating course", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("An unexpected error occurred while updating the course");
        }

    }

    public async Task<Response<CourseDTO>> UpdateCourse(int id, UpdateCourseDto updateCourseDto)
    {
        try
        {
            var course = await courseRepository.Get(id);
            if (course == null)
                return Response<CourseDTO>.NotFound("Course not found");

            course.Name = updateCourseDto.Name;
            course.Description = updateCourseDto.Description;

            var updatedCourse = await courseRepository.UpdateAndCommit(course);
            return Response<CourseDTO>.Ok(updatedCourse.ToDto(mapper));
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("Invalid operation while updating course", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("An unexpected error occurred while updating the course");
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
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<bool>.Fail("Invalid operation while deleting course", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<bool>.Fail("An unexpected error occurred while deleting the course");
        }
    }
}