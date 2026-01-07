using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Extentions.ModelExtentions;

public static class MapperExtensions
{
    #region DTO Mappers

    public static PlanningDTO ToDto(this Planning model, IMapper mapper)
    {
        return mapper.Map<PlanningDTO>(model);
    }

    public static LessonDTO ToDto(this Lesson lesson, IMapper mapper)
    {
        return mapper.Map<LessonDTO>(lesson);
    }

    public static CourseDto ToDto(this Course course, IMapper mapper)
    {
        return mapper.Map<CourseDto>(course);
    }

    public static Planning ToModel(this PlanningDTO dto, IMapper mapper)
    {
        return mapper.Map<Planning>(dto);
    }

    public static Course ToModel<T>(this T courseDto, IMapper mapper)
    {
        return mapper.Map<Course>(courseDto);
    }

    #endregion
}
