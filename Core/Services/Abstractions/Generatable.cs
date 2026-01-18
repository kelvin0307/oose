using Core.Common;
using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Domain.Enums;

namespace Core.Services.Abstractions;
public abstract class Generatable<Domain, Id>(IDocumentFactory documentFactory)
{
    // should contain the full flow for generating a document
    public abstract Task<Response<DocumentDto>> GenerateDocument(Id id, DocumentTypes documentType);

    /// <summary>
    /// Implement this method to map your domain model to DocumentDataDTO, 
    /// map the data to correct paragraphs within the object to generate.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    protected abstract DocumentDataDto MapToDocumentDataDTO(Domain document);

    protected DocumentDto CreateDocument(DocumentDataDto documentData, DocumentTypes documentType)
    {
        return documentFactory.GenerateDocument(documentData, documentType);
    }
}
