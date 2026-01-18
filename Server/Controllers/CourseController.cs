using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CourseController(ICourseService courseService, ILearningOutcomeService learningOutcomeService, IValidatorService validatorService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await courseService.GetAllCourses();
        return HandleResponse(response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await courseService.GetCourseById(id);
        return HandleResponse(response);
    }
    
        
    [HttpGet("{id}/learningOutcomes")]
    public async Task<IActionResult> GetLearningOutcomesByCourseId(int id)
    {
        var response = await learningOutcomeService.GetAllLearningOutcomesByCourseId(id);
        return HandleResponse(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
    {
        var response = await courseService.CreateCourse(createCourseDto);
        return HandleCreatedResponse(response, nameof(Get), new { id = response.Result?.Id });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        var response = await courseService.UpdateCourse(id, updateCourseDto);
        return HandleResponse(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await courseService.DeleteCourse(id);
        return HandleResponse(response, noContentOnSuccess: true);
    }

    [HttpGet("{id}/validatePlanning")]
    public async Task<IActionResult> ValidatePlanning(int id)
    {
        var response = await validatorService.ValidateCoursePlanning(id);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return HandleResponse(response, noContentOnSuccess: true);
    }


}