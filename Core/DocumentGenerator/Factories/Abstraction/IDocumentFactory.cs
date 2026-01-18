using Core.DTOs;
using Core.Enums;
using Domain.Enums;

namespace Core.DocumentGenerator.Factories.Abstraction;
public interface IDocumentFactory
{
    DocumentDto GenerateDocument(DocumentDataDto documentData, DocumentType documentType);
}
