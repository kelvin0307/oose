using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Domain.Enums;

namespace Core.DocumentGenerator.Factories;
public class DocumentFactory : IDocumentFactory
{
    public DocumentDto GenerateDocument(DocumentDataDto documentData, DocumentTypes documentType)
    {
        switch (documentType)
        {
            case DocumentTypes.Docx:
                var docxGenerator = new Generators.DocXGenerator();
                return docxGenerator.GenerateDocument(documentData);
            case DocumentTypes.Pdf:
                var pdfGenerator = new Generators.PdfGenerator();
                return pdfGenerator.GenerateDocument(documentData);
            case DocumentTypes.Csv:
                var csvGenerator = new Generators.CsvGenerator();
                return csvGenerator.GenerateDocument(documentData);
            default:
                throw new NotSupportedException($"Document type {documentType} is not supported.");
        }
    }
}
