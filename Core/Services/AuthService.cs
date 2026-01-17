using Core.Common;
using Core.DTOs;
using Data.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class AuthService(IRepository<Teacher> teacherRepository) : IAuthService
{
    public async Task<Response<TeacherLoginDTO>> LoginTeacherByEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Response<TeacherLoginDTO>.Fail("Email is required", ResponseStatus.ValidationError);
            }

            var teacher = await teacherRepository.Get(t => t.Email == email.Trim());

            if (teacher == null)
            {
                return Response<TeacherLoginDTO>.NotFound($"No teacher found with email: {email}");
            }

            var teacherLoginDto = new TeacherLoginDTO
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                MiddleName = teacher.MiddleName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                TeacherCode = teacher.TeacherCode
            };

            return Response<TeacherLoginDTO>.Ok(teacherLoginDto);
        }
        catch (Exception ex)
        {
            return Response<TeacherLoginDTO>.Fail($"An error occurred during login: {ex.Message}");
        }
    }
}
