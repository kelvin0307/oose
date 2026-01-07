using Domain.Models;

namespace Core.Extentions.ModelExtentions;

public static class CouseExtensions
{
    public static IQueryable<Lesson> GetByPlanningId(this IQueryable<Lesson> lessons, int planningId)
    {
        return lessons.Where(x => x.PlanningId == planningId);
    }
}
