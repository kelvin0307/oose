using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IGradeService
{
    Task<Response<GradeDTO>> CreateGrade(CreateGradeDTO createGradeDTO);
    Task<Response<GradeDTO>> UpdateGrade(UpdateGradeDTO updateGradeDTO);
    Task<Response<List<GradeDTO>>> GetLatestGradesByClassAndExecution(int classId, int courseExecutionId);
}
