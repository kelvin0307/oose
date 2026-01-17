using Core.Interfaces.Services;
using Core.DTOs.Imports.Nijmegen;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController(IImportService<NijmegenImportDataDto> nijmegenImportService) : BaseApiController
{
    [HttpPost("nijmegen")]
    public async Task<IActionResult> Import([FromBody] NijmegenImportDataDto data)
    {
        var response = await nijmegenImportService.Import(data);
        return HandleResponse(response);
    }    
}
