using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CourseExecutionController(ICourseExecutionService courseExecutionService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await courseExecutionService.GetAllCourseExecutions();
        return HandleResponse(response);
    }    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await courseExecutionService.GetCourseExecutionById(id);
        return HandleResponse(response);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseExecutionDto createCourseExecutionDto)
    {
        var response = await courseExecutionService.CreateCourseExecution(createCourseExecutionDto);
        return HandleCreatedResponse(response, nameof(Get), new { id = response.Result?.Id });
    }    
    [HttpPost("{id}/end")]
    public async Task<IActionResult> EndCourseExecution(int id)
    {
        var response = await courseExecutionService.EndCourseExecution(id);
        return HandleResponse(response);
    }
}