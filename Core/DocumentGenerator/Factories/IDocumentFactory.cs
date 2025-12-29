using Core.DTOs;
using Domain.Enums;

namespace Core.DocumentGenerator.Factories;
public interface IDocumentFactory
{
    byte[] GenerateDocument(DocumentDataDTO documentData, DocumentTypes documentType);
}
