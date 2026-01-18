using Core.DTOs;
using Domain.Enums;

namespace Core.DocumentGenerator.Factories.Abstraction;
public interface IDocumentFactory
{
    DocumentDto GenerateDocument(DocumentDataDto documentData, DocumentTypes documentType);
}
