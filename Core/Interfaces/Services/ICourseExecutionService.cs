using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface ICourseExecutionService
{
    Task<Response<List<CourseExecutionDTO>>> GetAllCourseExecutions();
    Task<Response<CourseExecutionDTO>> GetCourseExecutionById(int id);
    Task<Response<CourseExecutionDTO>> CreateCourseExecution(CreateCourseExecutionDto createCourseExectionDto);
    Task<Response<CourseExecutionDTO>> EndCourseExecution(int id);
}