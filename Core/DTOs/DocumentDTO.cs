namespace Core.DTOs;

public class DocumentDTO
{
    public byte[] Document { get; set; } = [];
    public string ContentType { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
}
