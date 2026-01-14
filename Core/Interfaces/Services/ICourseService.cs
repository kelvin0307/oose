using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ICourseService
    {
        Task<Response<List<CourseDTO>>> GetAllCourses();
        Task<Response<CourseDTO>> GetCourseById(int id);
        Task<Response<CourseDTO>> CreateCourse(CreateCourseDto createCourseDto);
        Task<Response<CourseDTO>> UpdateCourse(int id, UpdateCourseDto updateCourseDto);
        Task<Response<bool>> DeleteCourse(int id);
    }
}