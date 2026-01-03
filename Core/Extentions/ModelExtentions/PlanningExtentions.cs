using AutoMapper;
using Core.DTOs;
using Domain.Models;

namespace Core.Extentions.ModelExtentions;

public static class PlanningExtentions
{
    public static IQueryable<Planning> GetByCourseId(this IQueryable<Planning> plannings, int courseId)
    {
        return plannings.Where(x => x.CourseId == courseId);
    }
    public static PlanningDTO ToDto(this Planning model, IMapper mapper)
    {
        return mapper.Map<PlanningDTO>(model);
    }

    public static Planning ToModel(this PlanningDTO dto, IMapper mapper)
    {
        return mapper.Map<Planning>(dto);
    }
}
