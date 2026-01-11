using Domain.Models.Generalisation;

namespace Domain.Models;

public class Teacher : Person
{
    public string TeacherCode { get; set; } = string.Empty;
}

