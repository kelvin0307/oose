using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController(ILessonService lessonService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonDto createLessonDTO)
    {
        var result = await lessonService.CreateLesson(createLessonDTO);
        
        if (!result.Success)
            return HandleResponse(result);

        return HandleCreatedResponse(result, nameof(GetLessonById), new { lessonId = result.Result?.Id });
    }

    [HttpGet("{lessonId}")]
    public async Task<IActionResult> GetLessonById(int lessonId)
    {
        var result = await lessonService.GetLessonById(lessonId);
        return HandleResponse(result);
    }

    [HttpGet("planning/{planningId}")]
    public async Task<IActionResult> GetAllLessonsByPlanningId(int planningId)
    {
        var result = await lessonService.GetAllLessonsByPlanningId(planningId);
        return HandleResponse(result);
    }

    [HttpPut("{lessonId}")]
    public async Task<IActionResult> UpdateLesson(int lessonId, [FromBody] UpdateLessonDto updateLessonDTO)
    {
        updateLessonDTO.Id = lessonId;
        var result = await lessonService.UpdateLesson(updateLessonDTO);
        return HandleResponse(result);
    }

    [HttpDelete("{lessonId}")]
    public async Task<IActionResult> DeleteLesson(int lessonId)
    {
        var result = await lessonService.DeleteLesson(lessonId);
        return HandleResponse(result, noContentOnSuccess: true);
    }

    [HttpPost("{lessonId}/learning-outcomes")]
    public async Task<IActionResult> AddLearningOutcomesToLesson(int lessonId, [FromBody] IList<int> learningOutcomeIds)
    {
        var result = await lessonService.AddLearningOutcomesToLesson(lessonId, learningOutcomeIds);
        return HandleResponse(result, noContentOnSuccess: true);
    }

    [HttpDelete("{lessonId}/learning-outcomes")]
    public async Task<IActionResult> RemoveLearningOutcomesFromLesson(int lessonId, [FromBody] IList<int> learningOutcomeIds)
    {
        var result = await lessonService.RemoveLearningOutcomesFromLesson(lessonId, learningOutcomeIds);
        return HandleResponse(result, noContentOnSuccess: true);
    }
}
