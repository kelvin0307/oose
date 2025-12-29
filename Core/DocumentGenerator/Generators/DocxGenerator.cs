using Core.DTOs;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Core.DocumentGenerator.Generators;
public class DocXGenerator
{
    public byte[] GenerateDocument(DocumentDataDTO documentData)
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

            body.Append(CreateHeading(documentData.Title, 3));
            foreach (var paragraph in documentData.Paragraphs)
            {
                body.Append(CreateHeading(paragraph.Key, 4));
                body.Append(CreateParagraph(paragraph.Value));
            }

            mainPart.Document.Save();
        }
        return stream.ToArray();
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
    private static Paragraph CreateHeading(string text, int level = 1)
    {
        return new Paragraph(
            new ParagraphProperties(
                new ParagraphStyleId
                {
                    Val = $"Heading{level}"
                }
            ),
            new Run(new Text(text))
        );
    }
}
