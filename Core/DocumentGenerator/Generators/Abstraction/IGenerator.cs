using Core.DTOs;

namespace Core.DocumentGenerator.Generators.Abstraction;

public interface IGenerator
{
    string ContentType { get; }
    DocumentDTO GenerateDocument(DocumentDataDTO documentData);
}
