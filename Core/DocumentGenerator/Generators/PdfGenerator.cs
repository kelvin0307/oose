using Core.DocumentGenerator.Generators.Abstraction;
using Core.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Core.DocumentGenerator.Generators;

public class PdfGenerator : IGenerator
{
    public string ContentType => "application/pdf";

    public DocumentDto GenerateDocument(DocumentDataDto documentData)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Content().Column(column =>
                {
                    // Hoofdtitel
                    column.Item().PaddingBottom(20).Text(documentData.Title)
                        .FontSize(20)
                        .SemiBold();

                    foreach (var paragraph in documentData.Paragraphs)
                    {
                        // Key
                        column.Item()
                            .Text(paragraph.Key)
                            .SemiBold()
                            .FontSize(14);

                        // Value
                        column.Item().PaddingBottom(20)
                            .Text(paragraph.Value);
                    }
                });
            });
        });


        // saving to the server
        //File.WriteAllBytes("output.pdf", pdfBytes);

        return new DocumentDto
        {
            Document = document.GeneratePdf(),
            ContentType = ContentType,
            DocumentName = documentData.Title
        };
    }
}
