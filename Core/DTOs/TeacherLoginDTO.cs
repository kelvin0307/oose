namespace Core.DTOs;

public class TeacherLoginDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TeacherCode { get; set; } = string.Empty;
}
