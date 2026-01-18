using Core.Common;
using Core.DTOs;
using Core.Enums;
using Domain.Enums;

namespace Core.Interfaces.Services;

public interface IAssessmentDocumentService
{
    Task<Response<DocumentDto>> GenerateDocument(int testId, DocumentType documentType);
}
