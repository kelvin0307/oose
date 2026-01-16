namespace Domain.Models;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ClassCode { get; set; } = string.Empty;
    public IList<Student> Students { get; set; } = [];
}
