using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Extentions.ModelExtentions;
public static class CourseExtentions
{
    public static CourseDto ToDto(this Course course, IMapper mapper)
    {
        return mapper.Map<CourseDto>(course);
    }

    public static Course ToModel<T>(this T courseDto, IMapper mapper)
    {
        return mapper.Map<Course>(courseDto);
    }
}
