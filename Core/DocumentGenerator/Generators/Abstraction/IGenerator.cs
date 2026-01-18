using Core.DTOs;

namespace Core.DocumentGenerator.Generators.Abstraction;

public interface IGenerator
{
    string ContentType { get; }
    DocumentDto GenerateDocument(DocumentDataDto documentData);
}
