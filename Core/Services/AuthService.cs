using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class AuthService(IRepository<Teacher> teacherRepository) : IAuthService
{
    public async Task<Response<TeacherLoginDto>> LoginTeacherByEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Response<TeacherLoginDto>.Fail("Email is required", ResponseStatus.ValidationError);
            }

            var teacher = await teacherRepository.Get(t => t.Email == email.Trim());

            if (teacher == null)
            {
                return Response<TeacherLoginDto>.NotFound($"No teacher found with email: {email}");
            }

            var teacherLoginDto = new TeacherLoginDto
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                MiddleName = teacher.MiddleName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                TeacherCode = teacher.TeacherCode
            };

            return Response<TeacherLoginDto>.Ok(teacherLoginDto);
        }
        catch (Exception ex)
        {
            return Response<TeacherLoginDto>.Fail($"An error occurred during login: {ex.Message}");
        }
    }
}
