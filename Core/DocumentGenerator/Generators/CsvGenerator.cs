using System.Globalization;
using System.Text;
using Core.DocumentGenerator.Generators.Abstraction;
using Core.DTOs;
using CsvHelper;

namespace Core.DocumentGenerator.Generators;

public class CsvGenerator : IGenerator
{
    public string ContentType => "text/csv";

    public DocumentDTO GenerateDocument(DocumentDataDTO documentData)
    {


        using var ms = new MemoryStream();
        using (var writer = new StreamWriter(ms, leaveOpen: true, encoding: Encoding.UTF8))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteField(documentData.Title);
            csv.WriteRecords(documentData.Paragraphs.OrderBy(x => x.Value));
            writer.Flush();
        }

        return new DocumentDTO()
        {
            ContentType = ContentType,
            Document = ms.ToArray(),
            DocumentName = documentData.Title
        };
    }
}
