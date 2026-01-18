using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces.Services;

public interface IMaterialService
{
    Task<Response<DocumentDTO>> GenerateDocument(MaterialIdDTO materialId, DocumentTypes documentType);
    Task<Response<MaterialDTO>> CreateMaterial(CreateMaterialDTO createMaterialDTO);
    Task<Response<MaterialDTO>> UpdateMaterial(UpdateMaterialDTO updatedMaterial);
    Task<Response<IList<MaterialDTO>>> GetMaterialByLessonId(int lessonId);
    Task<Response<bool>> DeleteMaterial(int materialId);
}
