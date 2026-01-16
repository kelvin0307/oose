using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Extensions.ModelExtensions;

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

    public static CourseDTO ToDto(this Course course, IMapper mapper)
    {
        return mapper.Map<CourseDTO>(course);
    }
    
    public static RubricDto ToDto(this Rubric rubric, IMapper mapper)
    {
        return mapper.Map<RubricDto>(rubric);
    }

    public static Planning ToModel(this PlanningDTO dto, IMapper mapper)
    {
        return mapper.Map<Planning>(dto);
    }

    public static Course ToModel<T>(this T courseDto, IMapper mapper)
    {
        return mapper.Map<Course>(courseDto);
    }

    public static Rubric ToModel(this CreateRubricDto dto, IMapper mapper)
    {
        return mapper.Map<Rubric>(dto);
    }
    public static AssessmentDimension ToModel(this CreateAssessmentDimensionDto dto, IMapper mapper)
    {
        return mapper.Map<AssessmentDimension>(dto);
    }
    public static Rubric ToModel(this UpdateRubricDto dto, IMapper mapper)
    {
        return mapper.Map<Rubric>(dto);
    }
    #endregion
}