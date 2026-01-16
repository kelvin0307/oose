using Domain.Models.Generalisation;

namespace Domain.Models;

public class Student : Person
{
    public int StudentNumber { get; set; }
    public int? ClassId { get; set; }
    public Class? Class { get; set; }
    public IList<Grade> Grades { get; set; } = [];
}
