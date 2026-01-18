namespace Core.DTOs;

public class UpdateGradeDto
{
    public int Id { get; set; }
    public string? Grade { get; set; } = string.Empty;
    public string? Feedback { get; set; }
}
