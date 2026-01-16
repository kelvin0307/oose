using Data.Adapters.Nijmegen.DTOs;
using Domain.Models;

namespace Data.Adapters.Nijmegen.Mappers;

public static class NijmegenPlanningMapper
{
    public static Planning ToPlanning(NijmegenPlanningDto dto)
    {
        return new Planning
        {
            Id = dto.SysCode,

            Lessons = dto.Lessons?
                .Select(NijmegenLessonMapper.ToLesson)
                .ToList()
        };
    }
}
