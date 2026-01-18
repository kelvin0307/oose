using Core.DTOs;
using Core.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialController(IMaterialService materialService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialDto createMaterialdto)
    {
        var result = await materialService.CreateMaterial(createMaterialdto);

        if (!result.Success)
            return HandleResponse(result);

        return HandleCreatedResponse(result, nameof(GetMaterialByLessonId), new { lessonId = createMaterialdto.LessonId });
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<IActionResult> GetMaterialByLessonId(int lessonId)
    {
        var result = await materialService.GetMaterialByLessonId(lessonId);
        return HandleResponse(result);
    }

    [HttpPut("{materialId}")]
    public async Task<IActionResult> UpdateMaterial(int materialId, [FromBody] UpdateMaterialDto updatedMaterial)
    {
        updatedMaterial.Id = materialId;
        var result = await materialService.UpdateMaterial(updatedMaterial);
        return HandleResponse(result);
    }

    [HttpDelete("{materialId}")]
    public async Task<IActionResult> DeleteMaterial(int materialId)
    {
        var result = await materialService.DeleteMaterial(materialId);
        return HandleResponse(result, noContentOnSuccess: true);
    }

    [HttpPost("document/{documentType}")]
    public async Task<IActionResult> GenerateDocument([FromBody] MaterialIdDto materialId, DocumentTypes documentType) { 
        var doc = await materialService.GenerateDocument(materialId, documentType);

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
