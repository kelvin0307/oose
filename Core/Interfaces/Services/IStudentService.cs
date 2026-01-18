using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IStudentService
{
    Task<Response<StudentDto>> GetStudentById(int studentId);
}
