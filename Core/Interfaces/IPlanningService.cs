using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces;
public interface IPlanningService
{
    Response<PlanningDTO> GetPlanningByCourseId(int courseId);
    DocumentDTO GenerateDocument(int courseId, DocumentTypes documentType);

}
