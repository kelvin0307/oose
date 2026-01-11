using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces;

public interface IMaterialService
{
    Task<Response<DocumentDTO>> GenerateDocument(int courseId, DocumentTypes documentType);
}
