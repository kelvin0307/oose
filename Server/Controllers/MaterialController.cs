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


}
