using Domain.Models;

namespace Core.Extentions.ModelExtentions;

public static class PlanningExtensions
{
    public static IQueryable<Planning> GetByCourseId(this IQueryable<Planning> plannings, int courseId)
    {
        return plannings.Where(x => x.CourseId == courseId);
    }
}
