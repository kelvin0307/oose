using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Extensions.ModelExtensions;
using Core.Interfaces.Adapters;
using Data.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.DTOs.Imports.Nijmegen;
using Domain.Enums;
using Domain.Models;

namespace Core.Services;

public class ImportService<TImportDto>(IImportAdapter<TImportDto> adapter, IRepository<Course> courseRepository, IMapper mapper) : IImportService<TImportDto>
{
    public async Task<Response<CourseDTO>> Import(TImportDto data)
    {
        try
        {
            var course = adapter.GetMappedCourseData(data);
            if (course == null)
            {
                return Response<CourseDTO>.Fail("Could not create course, Invalid data");
            }
            
            //force course status to be concept. because a different systems truth. is not our truth
            course.Status = CourseStatus.Concept;

            var createdCourse = await courseRepository.CreateAndCommit(course);
            if (createdCourse == null)
            {
                return Response<CourseDTO>.Fail("Course could not be created");
            }
            return Response<CourseDTO>.Ok(createdCourse.ToDto(mapper));
        } 
        catch (InvalidOperationException)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("Invalid operation while deleting rubric", ResponseStatus.InvalidOperation);
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return Response<CourseDTO>.Fail("An unexpected error occurred while deleting the rubric");
        }
    }
}