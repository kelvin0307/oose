using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassController(IClassService classService) : BaseApiController
{
    [HttpGet("{classId}")]
    public async Task<IActionResult> GetClass(int classId)
    {
        var response = await classService.GetClassById(classId);
        return HandleResponse(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClasses()
    {
        var response = await classService.GetAllClasses();
        return HandleResponse(response);
    }

    [HttpGet("{classId}/students")]
    public async Task<IActionResult> GetStudentsByClass(int classId)
    {
        var response = await classService.GetStudentsByClassId(classId);
        return HandleResponse(response);
    }
}
