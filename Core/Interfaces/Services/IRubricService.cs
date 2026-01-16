using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IRubricService
{
    Task<Response<List<RubricDto>>> GetAllRubrics();
    Task<Response<List<RubricDto>>> GetRubricsByLearningOutcomeId(int courseId);
    Task<Response<RubricDto>> GetRubricById(int id);
    Task<Response<RubricDto>> CreateRubric(CreateRubricDto createRubricDto);
    Task<Response<RubricDto>> UpdateRubric(int id, UpdateRubricDto updateRubricDto);
    Task<Response<bool>> DeleteRubric(int id);
}