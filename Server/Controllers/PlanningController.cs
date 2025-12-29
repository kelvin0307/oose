using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanningController(IPlanningService planningService) : ControllerBase
{
    [HttpGet("{courseId}")]
    public async Task<IActionResult> Get(int courseId)
    {
        return Ok(await planningService.GetPlanningByCourseId(courseId));
    }

    [HttpGet("[action]/{courseId}")]
    public async Task<IActionResult> GenerateDocument(int courseId)
    {
        var doc = await planningService.GenerateDocument(courseId);
        return File(
            doc,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "rapport.docx"
        );
    }


}
