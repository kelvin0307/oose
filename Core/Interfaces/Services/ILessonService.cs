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
    // Bulk actions for learning outcomes
    Task<Response<bool>> AddLearningOutcomesToLesson(int lessonId, IList<int> learningOutcomeIds);
    Task<Response<bool>> RemoveLearningOutcomesFromLesson(int lessonId, IList<int> learningOutcomeIds);
}
