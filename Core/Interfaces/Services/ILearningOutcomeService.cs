using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface ILearningOutcomeService
{
    Task<Response<List<LearningOutcomeDto>>> GetAllLearningOutcomes();
    Task<Response<List<LearningOutcomeDto>>> GetAllLearningOutcomesByCourseId(int courseId);
    Task<Response<LearningOutcomeDto>> GetLearningOutcomeById(int id);
    Task<Response<LearningOutcomeDto>> CreateLearningOutcome(CreateLearningOutcomeDto createLearningOutcomeDto);
    Task<Response<LearningOutcomeDto>> UpdateLearningOutcome(int id, UpdateLearningOutcomeDto updateLearningOutcome);
    Task<Response<bool>> DeleteLearningOutcome(int id);
}