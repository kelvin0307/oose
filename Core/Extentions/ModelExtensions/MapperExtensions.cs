using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Extensions.ModelExtensions;

public static class MapperExtensions
{
    #region DTO Mappers

    public static PlanningDto ToDto(this Planning model, IMapper mapper)
    {
        return mapper.Map<PlanningDto>(model);
    }

    public static LessonDto ToDto(this Lesson lesson, IMapper mapper)
    {
        return mapper.Map<LessonDto>(lesson);
    }

    public static CourseDto ToDto(this Course course, IMapper mapper)
    {
        return mapper.Map<CourseDto>(course);
    }
    
    public static RubricDto ToDto(this Rubric rubric, IMapper mapper)
    {
        return mapper.Map<RubricDto>(rubric);
    }

    public static Planning ToModel(this PlanningDto dto, IMapper mapper)
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

    public static CourseExecutionDto ToDto(this CourseExecution courseExecution, IMapper mapper)
    {
        return mapper.Map<CourseExecutionDto>(courseExecution);
    }
    #endregion
}