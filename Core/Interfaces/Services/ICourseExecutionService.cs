using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface ICourseExecutionService
{
    Task<Response<List<CourseExecutionDto>>> GetAllCourseExecutions();
    Task<Response<CourseExecutionDto>> GetCourseExecutionById(int id);
    Task<Response<CourseExecutionDto>> CreateCourseExecution(CreateCourseExecutionDto createCourseExectionDto);
    Task<Response<CourseExecutionDto>> EndCourseExecution(int id);
}