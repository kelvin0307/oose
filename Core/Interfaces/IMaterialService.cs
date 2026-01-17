using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces;

public interface IMaterialService
{
    Task<Response<DocumentDTO>> GenerateDocument(int courseId, DocumentTypes documentType);
    Task<Response<MaterialDTO>> CreateMaterial(CreateMaterialDTO createMaterialDTO);
    Task<Response<MaterialDTO>> UpdateMaterial(UpdateMaterialDTO updatedMaterial);
    Task<Response<IList<MaterialDTO>>> GetMaterialByLessonId(int lessonId);
    Task<Response<bool>> DeleteMaterial(int materialId);
}
