using Domain.Enums;

namespace Core.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CourseStatus Status { get; set; }
}

public class CreateCourseDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class UpdateCourseDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}
