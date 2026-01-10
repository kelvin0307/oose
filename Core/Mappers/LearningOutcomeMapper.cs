using Core.DTOs;
using Domain.Models;

namespace Core.Mappers;

public static class LearningOutcomeMapper
{
    public static LearningOutcomeDto ToDto(LearningOutcome learningOutcome) => new()
    {
        Id = learningOutcome.Id,
        Name = learningOutcome.Name,
        Description = learningOutcome.Description,
        EndQualification = learningOutcome.EndQualification,
        CourseId = learningOutcome.CourseId,
    };
}