using Core.DTOs;
using Core.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialController(IMaterialService materialService) : BaseApiController
{
    [HttpGet("[action]/{materialId}/{documentType}")]
    public async Task<IActionResult> GenerateDocument(int materialId, DocumentTypes documentType)
    {
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
    [HttpPut]
    public async Task<IActionResult> UpdateMaterial([FromBody] UpdateMaterialDTO updatedMaterial)
    {
        var result = await materialService.UpdateMaterial(updatedMaterial);
        return HandleResponse(result);
    }
    [HttpPost]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialDTO createMaterialdto)
    {
        var result = await materialService.CreateMaterial(createMaterialdto);
        return HandleResponse(result);
    }
    [HttpGet("Lesson/{lessonId}")]
    public async Task<IActionResult> GetAllByLessonId(int lessonId)
    {
        var result = await materialService.GetMaterialByLessonId(lessonId);
        return HandleResponse(result);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteMaterial([FromQuery] int materialId)
    {
        var result = await materialService.DeleteMaterial(materialId);
        return HandleResponse(result);
    }
}
