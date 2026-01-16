using Data.Adapters.Nijmegen.DTOs;
using Data.Adapters.Nijmegen.Mappers;
using Data.Interfaces;
using Domain.Models;
using System.Net.Http.Json;

namespace Data.Adapters.Nijmegen;

public class NijmegenLearningOutcomeAdapter : IDataSource<LearningOutcome>
{
    private readonly HttpClient _http;

    public NijmegenLearningOutcomeAdapter(HttpClient http)
    {
        _http = http;
    }

    public virtual async Task<IEnumerable<LearningOutcome>> GetAllAsync()
    {
        var dtos = await _http.GetFromJsonAsync<List<NijmegenLearningOutcomeDto>>("learning-outcomes")
                   ?? new List<NijmegenLearningOutcomeDto>();

        return dtos.Select(NijmegenLearningOutcomeMapper.ToLearningOutcome);
    }
}
