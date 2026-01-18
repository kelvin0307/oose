using Core.Enums;
using Core.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanningController(IPlanningService planningService) : BaseApiController
{
    [HttpGet("{courseId}")]
    public IActionResult GetPlanningByCourseId(int courseId)
    {
        return Ok(planningService.GetPlanningByCourseId(courseId));
    }

    [HttpGet("{courseId}/document/{documentType}")]
    public async Task<IActionResult> GenerateDocument(int courseId, DocumentType documentType)
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
