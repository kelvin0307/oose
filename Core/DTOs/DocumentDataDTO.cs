namespace Core.DTOs;
public class DocumentDataDTO
{
    public string Title { get; set; } = string.Empty;
    public IDictionary<string, string>? Paragraphs { get; set; } = new Dictionary<string, string>();
}
