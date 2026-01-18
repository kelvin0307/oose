using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IGradeService
{
    Task<Response<GradeDto>> CreateGrade(CreateGradeDto createGradeDTO);
    Task<Response<GradeDto>> UpdateGrade(UpdateGradeDto updateGradeDTO);
    Task<Response<List<GradeDto>>> GetLatestGradesByClassAndExecution(int classId, int courseExecutionId);
}
