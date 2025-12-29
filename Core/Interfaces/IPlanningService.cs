using Core.DTOs;

namespace Core.Interfaces;
public interface IPlanningService
{
    Task<PlanningDTO?> GetPlanningByCourseId(int courseId);
    Task<byte[]> GenerateDocument(int id);

}
