using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces.Services;
public interface IPlanningService
{
    Response<PlanningDTO> GetPlanningByCourseId(int courseId);
    Task<Response<DocumentDTO>> GenerateDocument(int courseId, DocumentTypes documentType);

}
