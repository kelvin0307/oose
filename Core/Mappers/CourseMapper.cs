using Core.DTOs;
using Domain.Models;

namespace Core.Mappers;

public static class CourseMapper
{
    public static CourseDto ToDto(Course course) => new()
    {
        Id = course.Id,
        Name = course.Name,
        Description = course.Description,
        Status = course.Status
    };
}