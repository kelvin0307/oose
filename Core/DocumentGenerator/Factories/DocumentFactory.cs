using Core.DTOs;
using Domain.Enums;

namespace Core.DocumentGenerator.Factories;
public class DocumentFactory : IDocumentFactory
{
    public byte[] GenerateDocument(DocumentDataDTO documentData, DocumentTypes documentType)
    {
        switch (documentType)
        {
            case DocumentTypes.Docx:
                var docxGenerator = new Generators.DocXGenerator();
                return docxGenerator.GenerateDocument(documentData);
            // Future cases for other document types can be added here
            default:
                throw new NotSupportedException($"Document type {documentType} is not supported.");
        }
    }
}
