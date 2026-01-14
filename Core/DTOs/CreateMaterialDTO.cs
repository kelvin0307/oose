namespace Core.DTOs;

public class CreateMaterialDTO
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int LessonId { get; set; }
}
