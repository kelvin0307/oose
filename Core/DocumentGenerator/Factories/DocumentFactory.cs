using Core.DocumentGenerator.Factories.Abstraction;
using Core.DTOs;
using Core.Enums;
using Domain.Enums;

namespace Core.DocumentGenerator.Factories;
public class DocumentFactory : IDocumentFactory
{
    public DocumentDto GenerateDocument(DocumentDataDto documentData, DocumentType documentType)
    {
        switch (documentType)
        {
            case DocumentType.Docx:
                var docxGenerator = new Generators.DocXGenerator();
                return docxGenerator.GenerateDocument(documentData);
            case DocumentType.Pdf:
                var pdfGenerator = new Generators.PdfGenerator();
                return pdfGenerator.GenerateDocument(documentData);
            case DocumentType.Csv:
                var csvGenerator = new Generators.CsvGenerator();
                return csvGenerator.GenerateDocument(documentData);
            default:
                throw new NotSupportedException($"Document type {documentType} is not supported.");
        }
    }
}
