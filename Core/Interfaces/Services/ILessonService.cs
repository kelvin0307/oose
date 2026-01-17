using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces.Services;

public interface ILessonService
{
    Task<Response<LessonDTO>> CreateLesson(CreateLessonDTO createLessonDTO);
    Task<Response<LessonDTO>> GetLessonById(int lessonId);
    Task<Response<IList<LessonDTO>>> GetAllLessonsByPlanningId(int planningId);
    Task<Response<LessonDTO>> UpdateLesson(UpdateLessonDTO updateLessonDTO);
    Task<Response<bool>> DeleteLesson(int lessonId);

    // New methods to couple/decouple learning outcomes from lessons
    Task<Response<bool>> AddLearningOutcomeToLesson(int lessonId, int learningOutcomeId);
    Task<Response<bool>> RemoveLearningOutcomeFromLesson(int lessonId, int learningOutcomeId);
}
