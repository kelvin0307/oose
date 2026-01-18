using Core.Common;
using Core.DTOs;
using Domain.Enums;

namespace Core.Interfaces.Services;

public interface ILessonService
{
    Task<Response<LessonDto>> CreateLesson(CreateLessonDto createLessonDTO);
    Task<Response<LessonDto>> GetLessonById(int lessonId);
    Task<Response<IList<LessonDto>>> GetAllLessonsByPlanningId(int planningId);
    Task<Response<LessonDto>> UpdateLesson(UpdateLessonDto updateLessonDTO);
    Task<Response<bool>> DeleteLesson(int lessonId);
    // Bulk actions for learning outcomes
    Task<Response<bool>> AddLearningOutcomesToLesson(int lessonId, IList<int> learningOutcomeIds);
    Task<Response<bool>> RemoveLearningOutcomesFromLesson(int lessonId, IList<int> learningOutcomeIds);
}
