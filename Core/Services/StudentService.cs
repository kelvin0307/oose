using AutoMapper;
using Core.Common;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class StudentService(IRepository<Class> classRepository, IRepository<Student> studentRepository, IMapper mapper) : IStudentService
{
    public async Task<Response<StudentDto>> GetStudentById(int studentId)
    {
        try
        {
            var student = await studentRepository.Get(studentId);
            if (student == null)
                return Response<StudentDto>.NotFound("Student not found in this class");

            var studentDto = mapper.Map<StudentDto>(student);
            return Response<StudentDto>.Ok(studentDto);
        }
        catch (InvalidOperationException)
        {
            return Response<StudentDto>.Fail("Invalid operation while fetching student", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<StudentDto>.Fail("An unexpected error occurred while fetching the student");
        }
    }
}
