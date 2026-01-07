using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces;
public abstract class Generatable<Domain>(IDocumentFactory documentFactory)
{
    // should contain the full flow for generating a document
    public abstract DocumentDTO GenerateDocument(int id, DocumentTypes documentType);

    /// <summary>
    /// Implement this method to map your domain model to DocumentDataDTO, 
    /// map the data to correct paragraphs within the object to generate.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    protected abstract DocumentDataDTO MapToDocumentDataDTO(Domain document);

    protected DocumentDTO CreateDocument(DocumentDataDTO documentData, DocumentTypes documentType)
    {
        return documentFactory.GenerateDocument(documentData, documentType);
    }
}
