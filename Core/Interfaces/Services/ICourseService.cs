using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ICourseService
    {
        Task<Response<List<CourseDto>>> GetAllCourses();
        Task<Response<CourseDto>> GetCourseById(int id);
        Task<Response<CourseDto>> CreateCourse(CreateCourseDto createCourseDto);
        Task<Response<CourseDto>> UpdateCourse(int id, UpdateCourseDto updateCourseDto);

    }
}