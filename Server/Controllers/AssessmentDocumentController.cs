using Core.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentDocumentController(IAssessmentDocumentService assessmentDocumentService) : BaseApiController
    {
        [HttpGet("{testId}/document/{documentType}")]
        public async Task<IActionResult> GenerateDocument(int testId, DocumentTypes documentType)
        {
            var doc = await assessmentDocumentService.GenerateDocument(testId, documentType);

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
}
