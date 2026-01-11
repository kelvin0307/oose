using Core.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanningController(IPlanningService planningService) : BaseApiController
{
    [HttpGet("{courseId}")]
    public IActionResult Get(int courseId)
    {
        return Ok(planningService.GetPlanningByCourseId(courseId));
    }

    [HttpGet("[action]/{courseId}/{documentType}")]
    public async Task<IActionResult> GenerateDocument(int courseId, DocumentTypes documentType)
    {
        var doc = await planningService.GenerateDocument(courseId, documentType);

        if (!doc.Success)
        {
            return HandleResponse(doc);
        }

        return File(
            doc.Result.Document,
            doc.Result.ContentType,
            doc.Result.DocumentName
        );
    }
}
