using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradeController(IGradeService gradeService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeDTO createGradeDTO)
    {
        var response = await gradeService.CreateGrade(createGradeDTO);
        if (!response.Success)
            return HandleResponse(response);

        return CreatedAtAction(nameof(GetLatestGradesByClassAndExecution), new { classId = 0, courseExecutionId = response.Result?.CourseExecutionId ?? 0 }, response.Result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeDTO updateGradeDTO)
    {
        updateGradeDTO.Id = id;
        var response = await gradeService.UpdateGrade(updateGradeDTO);
        return HandleResponse(response);
    }

    [HttpGet("class/{classId}/execution/{courseExecutionId}")]
    public async Task<IActionResult> GetLatestGradesByClassAndExecution(int classId, int courseExecutionId)
    {
        var response = await gradeService.GetLatestGradesByClassAndExecution(classId, courseExecutionId);
        return HandleResponse(response);
    }
}
