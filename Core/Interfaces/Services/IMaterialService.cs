using Core.Common;
using Core.DTOs;
using Core.Enums;
using Domain.Enums;

namespace Core.Interfaces.Services;

public interface IMaterialService
{
    Task<Response<DocumentDto>> GenerateDocument(MaterialIdDto materialId, DocumentType documentType);
    Task<Response<MaterialDto>> CreateMaterial(CreateMaterialDto createMaterialDTO);
    Task<Response<MaterialDto>> UpdateMaterial(UpdateMaterialDto updatedMaterial);
    Task<Response<IList<MaterialDto>>> GetMaterialByLessonId(int lessonId);
    Task<Response<bool>> DeleteMaterial(int materialId);
}
