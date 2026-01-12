using Core.Common;
using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IAuthService
{
    Task<Response<TeacherLoginDTO>> LoginTeacherByEmail(string email);
}
