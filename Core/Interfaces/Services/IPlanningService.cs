using Core.Common;
using Core.DTOs;
using Core.Enums;
using Domain.Enums;

namespace Core.Interfaces.Services;
public interface IPlanningService
{
    Response<PlanningDto> GetPlanningByCourseId(int courseId);
    Task<Response<DocumentDto>> GenerateDocument(int courseId, DocumentType documentType);

}
