using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IClassService
{
    Task<Response<ClassDto>> GetClassById(int classId);
    Task<Response<List<ClassDto>>> GetAllClasses();
    Task<Response<List<StudentDto>>> GetStudentsByClassId(int classId);
}
