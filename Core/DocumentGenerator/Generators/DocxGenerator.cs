using Core.DocumentGenerator.Generators.Abstraction;
using Core.DTOs;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Core.DocumentGenerator.Generators;
public class DocXGenerator : IGenerator
{
    public string ContentType => "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    public DocumentDto GenerateDocument(DocumentDataDto documentData)
    {
        using var stream = new MemoryStream();

        using (var wordDoc = WordprocessingDocument.Create(
            stream,
            WordprocessingDocumentType.Document,
            true))
        {
            var mainPart = wordDoc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            body.Append(CreateParagraph(documentData.Title, true));
            foreach (var paragraph in documentData.Paragraphs)
            {
                body.Append(CreateParagraph(paragraph.Key, true));
                body.Append(CreateParagraph(paragraph.Value));
            }

            mainPart.Document.Save();
        }

        // saving to the server
        //File.WriteAllBytes(documentData.Title, stream.ToArray());

        return new DocumentDto
        {
            Document = stream.ToArray(),
            ContentType = ContentType,
            DocumentName = documentData.Title
        };
    }

    private static Paragraph CreateParagraph(string text, bool bold = false)
    {
        return new Paragraph(
            new Run(
                new RunProperties(
                    bold ? [new Bold()] : []
                ),
                new Text(text)
            )
        );
    }
}
