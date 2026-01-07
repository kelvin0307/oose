namespace Core.DTOs;
public class DocumentDataDTO
{
    public string Title { get; set; } = "PlaceHolderText";
    public IDictionary<string, string> Paragraphs { get; set; } = new Dictionary<string, string>();
}
