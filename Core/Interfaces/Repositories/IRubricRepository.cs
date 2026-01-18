using Domain.Models;

namespace Core.Interfaces.Repositories
{
    public interface IRubricRepository : IRepository<Rubric>
    {
        Task<Rubric?> GetAggregate(int id);
        Task<List<Rubric>> GetAggregatesByLearningOutcomeId(int learningOutcomeId);
        Task<List<Rubric>> GetAggregatesByLearningOutcomeIds(IList<int> learningOutcomeIds);
        Task SaveAggregate();
    }
}