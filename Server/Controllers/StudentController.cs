using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(IStudentService studentService) : BaseApiController
    {
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudent(int studentId)
        {
            var response = await studentService.GetStudentById(studentId);
            return HandleResponse(response);
        }
    }
}
