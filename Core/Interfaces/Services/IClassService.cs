using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IClassService
{
    Task<Response<ClassDTO>> GetClassById(int classId);
    Task<Response<List<ClassDTO>>> GetAllClasses();
    Task<Response<List<StudentDTO>>> GetStudentsByClassId(int classId);
}
