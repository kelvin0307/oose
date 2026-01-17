using AutoMapper;
using Core.Common;
using Core.DTOs;
using Data.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Models;

namespace Core.Services;

public class StudentService(IRepository<Class> classRepository, IRepository<Student> studentRepository, IMapper mapper) : IStudentService
{
    public async Task<Response<StudentDTO>> GetStudentById(int studentId)
    {
        try
        {
            var student = await studentRepository.Get(studentId);
            if (student == null)
                return Response<StudentDTO>.NotFound("Student not found in this class");

            var studentDto = mapper.Map<StudentDTO>(student);
            return Response<StudentDTO>.Ok(studentDto);
        }
        catch (InvalidOperationException)
        {
            return Response<StudentDTO>.Fail("Invalid operation while fetching student", ResponseStatus.InvalidOperation);
        }
        catch (Exception)
        {
            return Response<StudentDTO>.Fail("An unexpected error occurred while fetching the student");
        }
    }
}
