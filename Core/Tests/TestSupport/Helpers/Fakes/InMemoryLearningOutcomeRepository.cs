using Core.Interfaces.Repositories;
using Domain.Models;
using System.Linq.Expressions;

namespace Core.Tests.TestSupport.Helpers.Fakes;

public class InMemoryLearningOutcomeRepository : IRepository<LearningOutcome>
{
    private readonly Func<List<LearningOutcome>> _getData;

    public InMemoryLearningOutcomeRepository(Func<List<LearningOutcome>> getData)
    {
        _getData = getData;
    }

    public IQueryable<LearningOutcome> Include<TProperty>(Expression<Func<LearningOutcome, TProperty>> navigationProperty)
    {
        return _getData().AsQueryable();
    }

    public IQueryable<LearningOutcome> Find(Expression<Func<LearningOutcome, bool>> predicate)
    {
        return _getData().AsQueryable().Where(predicate);
    }

    // IQueryable implementatie
    public Type ElementType => _getData().AsQueryable().ElementType;
    public Expression Expression => _getData().AsQueryable().Expression;
    public IQueryProvider Provider => _getData().AsQueryable().Provider;
    public System.Collections.IEnumerator GetEnumerator() => _getData().GetEnumerator();
    IEnumerator<LearningOutcome> IEnumerable<LearningOutcome>.GetEnumerator() => _getData().GetEnumerator();

    // Andere methoden niet gebruikt in tests
    public Task<LearningOutcome> CreateAndCommit(LearningOutcome entity) => throw new NotImplementedException();
    public void Create(List<LearningOutcome> entity) => throw new NotImplementedException();
    public void Delete(int id) => throw new NotImplementedException();
    public Task<LearningOutcome> UpdateAndCommit(LearningOutcome entity) => throw new NotImplementedException();
    public void Update(LearningOutcome entity) => throw new NotImplementedException();
    public Task<LearningOutcome> DeleteAndCommit(int id) => throw new NotImplementedException();
    public Task<LearningOutcome?> Get(int id) => throw new NotImplementedException();
    public Task<LearningOutcome?> Get(Expression<Func<LearningOutcome, bool>> action) => throw new NotImplementedException();
    public Task<List<LearningOutcome>> GetAll() => throw new NotImplementedException();
    public Task<List<LearningOutcome>> GetAll(Expression<Func<LearningOutcome, bool>> action) => throw new NotImplementedException();
    public Task SaveManually() => throw new NotImplementedException();
    public Task<List<LearningOutcome>> ToListAsync(IQueryable<LearningOutcome> query) => throw new NotImplementedException();
    public Task<LearningOutcome?> FirstOrDefaultAsync(IQueryable<LearningOutcome> query) => throw new NotImplementedException();
}
