using Data.Adapters.Nijmegen.DTOs;
using Data.Adapters.Nijmegen.Mappers;
using Data.Interfaces;
using Domain.Models;
using System.Net.Http.Json;

namespace Data.Adapters.Nijmegen;

public class NijmegenCourseAdapter : IDataSource<Course>
{
    private readonly HttpClient _http;

    public NijmegenCourseAdapter(HttpClient http)
    {
        _http = http;
    }

    public virtual async Task<IEnumerable<Course>> GetAllAsync()
    {
        var dtos = await _http.GetFromJsonAsync<List<NijmegenCourseDto>>("courses")
                   ?? new List<NijmegenCourseDto>();

        return dtos.Select(NijmegenCourseMapper.ToCourse);
    }
}
