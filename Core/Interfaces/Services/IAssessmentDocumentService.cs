using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces.Services;

public interface IAssessmentDocumentService
{
    Task<Response<DocumentDto>> GenerateDocument(int testId, DocumentTypes documentType);
}
