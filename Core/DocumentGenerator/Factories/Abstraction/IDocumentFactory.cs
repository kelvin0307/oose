using Core.DTOs;
using Domain.Enums;

namespace Core.DocumentGenerator.Factories.Abstraction;
public interface IDocumentFactory
{
    DocumentDTO GenerateDocument(DocumentDataDTO documentData, DocumentTypes documentType);
}
