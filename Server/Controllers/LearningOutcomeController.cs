using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningOutcomeController(ILearningOutcomeService learningOutcomeService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await learningOutcomeService.GetAllLearningOutcomes();
        return HandleResponse(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLearningOutcomeDto createLearningOutcomeDto)
    {
        var response = await learningOutcomeService.CreateLearningOutcome(createLearningOutcomeDto);
        return HandleCreatedResponse(response, nameof(GetAll), new { id = response.Result?.Id });
        // change nameof(GetAll) to nameof(Get) if GetById method is implemented
    }
}