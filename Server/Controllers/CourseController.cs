using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CourseController(ICourseService courseService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await courseService.GetAllCourses();
        return HandleResponse(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
    {
        var response = await courseService.CreateCourse(createCourseDto);
        return HandleCreatedResponse(response, "TODO: ADD ACTION URL ONCE GET CALL GETS BUILD", new { id = response.Result?.Id });
    }
}