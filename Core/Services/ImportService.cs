using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Adapters;
using Core.Interfaces.Services;
using Core.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Core.Extensions.Mapper;

namespace Core.Services;

public class ImportService<TImportDto>(IImportAdapter<TImportDto> adapter, IRepository<Course> courseRepository, IMapper mapper) : IImportService<TImportDto>
{
    public async Task<Response<CourseDto>> Import(TImportDto data)
    {
        try
        {
            var course = adapter.GetMappedCourseData(data);
            if (course == null)
            {
                return Response<CourseDto>.Fail("Could not create course, Invalid data");
            }
            
            //force course status to be concept. because a different systems truth. is not our truth
            course.Status = CourseStatus.Concept;

            var createdCourse = await courseRepository.CreateAndCommit(course);
            if (createdCourse == null)
            {
                return Response<CourseDto>.Fail("Course could not be created");
            }
            return Response<CourseDto>.Ok(createdCourse.ToDto(mapper));
        }
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("Invalid operation while deleting rubric", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            //TODO: Log exception
            return Response<CourseDto>.Fail("An unexpected error occurred while deleting the rubric");
        }
    }
}