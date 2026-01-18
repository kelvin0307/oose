using Domain.Enums;

namespace Core.DTOs;

public class CourseDTO
{
    public int Id { get; set; }
    public double EuropeanCredits { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CourseStatus Status { get; set; }
}