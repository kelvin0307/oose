using Data.Adapters.Nijmegen.DTOs;
using Domain.Models;

namespace Data.Adapters.Nijmegen.Mappers;

public static class NijmegenLessonMapper
{
    public static Lesson ToLesson(NijmegenLessonDto dto)
    {
        return new Lesson
        {
            Id = dto.SysCode,
            WeekNumber = dto.Weeknummer,
            Name = dto.Naam,
            SequenceNumber = dto.SequenceNumber,
            TestType = dto.TestVariant,

            LearningOutcomes = dto.Leeruitkomsten
                .Select(NijmegenLearningOutcomeMapper.ToLearningOutcome)
                .ToList()
        };
    }
}
