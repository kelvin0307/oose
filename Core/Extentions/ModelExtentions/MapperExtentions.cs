using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Extentions.ModelExtentions;

public static class MapperExtentions
{
    #region Queryable Filters
    public static IQueryable<Planning> GetByCourseId(this IQueryable<Planning> plannings, int courseId)
    {
        return plannings.Where(x => x.CourseId == courseId);
    }
    public static IQueryable<Lesson> GetByPlanningId(this IQueryable<Lesson> lessons, int planningId)
    {
        return lessons.Where(x => x.PlanningId == planningId);
    }
    #endregion

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
