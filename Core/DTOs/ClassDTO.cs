namespace Core.DTOs;

public class ClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ClassCode { get; set; } = string.Empty;
    public List<StudentDto>? Students { get; set; }
}
