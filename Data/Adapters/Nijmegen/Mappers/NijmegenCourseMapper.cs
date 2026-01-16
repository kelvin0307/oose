using Data.Adapters.Nijmegen.DTOs;
using Domain.Models;

namespace Data.Adapters.Nijmegen.Mappers;

public static class NijmegenCourseMapper
{
    public static Course ToCourse(NijmegenCourseDto dto)
    {
        return new Course
        {
            Id = dto.SysCode,
            Name = dto.Naam ?? string.Empty,
            Description = dto.Beschrijving ?? string.Empty,
            Status = (Domain.Enums.CourseStatus)dto.Status,

            Planning = dto.Planning != null
                ? NijmegenPlanningMapper.ToPlanning(dto.Planning)
                : null,

            LearningOutcomes = dto.Leeruitkomsten
                .Select(NijmegenLearningOutcomeMapper.ToLearningOutcome)
                .ToList()
        };
    }
}
