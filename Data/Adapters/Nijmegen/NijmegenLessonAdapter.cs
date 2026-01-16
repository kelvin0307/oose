using Data.Adapters.Nijmegen.DTOs;
using Data.Adapters.Nijmegen.Mappers;
using Data.Interfaces;
using Domain.Models;
using System.Net.Http.Json;

namespace Data.Adapters.Nijmegen;

public class NijmegenLessonAdapter : IDataSource<Lesson>
{
    private readonly HttpClient _http;

    public NijmegenLessonAdapter(HttpClient http)
    {
        _http = http;
    }

    public virtual async Task<IEnumerable<Lesson>> GetAllAsync()
    {
        var dtos = await _http.GetFromJsonAsync<List<NijmegenLessonDto>>("lessons")
                   ?? new List<NijmegenLessonDto>();

        return dtos.Select(NijmegenLessonMapper.ToLesson);
    }
}
