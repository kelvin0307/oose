using Core.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanningController(IPlanningService planningService) : ControllerBase
{
    [HttpGet("{courseId}")]
    public IActionResult Get(int courseId)
    {
        return Ok(planningService.GetPlanningByCourseId(courseId));
    }

    [HttpGet("[action]/{courseId}/{DocumentType}")]
    public IActionResult GenerateDocument(int courseId, DocumentTypes DocumentType)
    {
        var doc = planningService.GenerateDocument(courseId, DocumentType);
        return File(
            doc.Document,
            doc.ContentType,
            doc.DocumentName
        );
    }


}
