using Data.Adapters.Nijmegen.DTOs;
using Domain.Models;

namespace Data.Adapters.Nijmegen.Mappers;

public static class NijmegenLearningOutcomeMapper
{
    public static LearningOutcome ToLearningOutcome(NijmegenLearningOutcomeDto dto)
    {
        return new LearningOutcome
        {
            Id = dto.SysCode,
            Name = dto.Naam,
            Description = dto.Beschrijving,
            EndQualification = dto.Eindkwalificatie
        };
    }
}
