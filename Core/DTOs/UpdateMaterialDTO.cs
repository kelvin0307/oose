namespace Core.DTOs;

public class UpdateMaterialDto
{
    public int Id { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

}
