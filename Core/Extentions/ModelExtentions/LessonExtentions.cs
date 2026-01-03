using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Extentions.ModelExtentions;

public static class LessonExtentions
{
    public static IQueryable<Lesson> GetByPlanningId(this IQueryable<Lesson> lessons, int planningId)
    {
        return lessons.Where(x => x.PlanningId == planningId);
    }

    public static LessonDTO ToDto(this Lesson lesson, IMapper mapper)
    {
        return mapper.Map<LessonDTO>(lesson);
    }
}
