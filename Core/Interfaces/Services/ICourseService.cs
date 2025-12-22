using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ICourseService
    {
        Task<Response<List<CourseDto>>> GetAllCourses();
        Task<Response<CourseDto>> CreateCourse(CreateCourseDto createCourseDto);
    }
}