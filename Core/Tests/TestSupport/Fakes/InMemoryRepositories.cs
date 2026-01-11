using System.Linq.Expressions;
using Core.Interfaces.Repositories;
using Domain.Models;

namespace Core.Tests.TestSupport.Fakes;

/// <summary>
/// In-memory fake repository for Course testing
/// </summary>
public class InMemoryCourseRepository : IRepository<Course>
{
    private readonly Func<List<Course>> _getData;

    public InMemoryCourseRepository(Func<List<Course>> getData)
    {
        _getData = getData;
    }

    public IQueryable<Course> Include<TProperty>(Expression<Func<Course, TProperty>> navigationProperty)
    {
        return _getData().AsQueryable();
    }

    public IQueryable<Course> Find(Expression<Func<Course, bool>> predicate)
    {
        return _getData().AsQueryable().Where(predicate);
    }

    public Type ElementType => _getData().AsQueryable().ElementType;
    public Expression Expression => _getData().AsQueryable().Expression;
    public IQueryProvider Provider => _getData().AsQueryable().Provider;
    public System.Collections.IEnumerator GetEnumerator() => _getData().GetEnumerator();
    IEnumerator<Course> IEnumerable<Course>.GetEnumerator() => _getData().GetEnumerator();

    public Task<Course> CreateAndCommit(Course entity)
    {
        var courses = _getData();
        entity.Id = courses.Count + 1;
        courses.Add(entity);
        return Task.FromResult(entity);
    }

    public void Create(List<Course> entity) => throw new NotImplementedException();
    public void Delete(int id)
    {
        var courses = _getData();
        var course = courses.FirstOrDefault(c => c.Id == id);
        if (course != null)
            courses.Remove(course);
    }

    public Task<Course> UpdateAndCommit(Course entity)
    {
        var courses = _getData();
        var existing = courses.FirstOrDefault(c => c.Id == entity.Id);
        if (existing != null)
        {
            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.Status = entity.Status;
        }
        return Task.FromResult(entity);
    }

    public void Update(Course entity) => throw new NotImplementedException();
    public Task<Course> DeleteAndCommit(int id)
    {
        var courses = _getData();
        var course = courses.FirstOrDefault(c => c.Id == id);
        if (course != null)
            courses.Remove(course);
        return Task.FromResult(course!);
    }

    public Task<Course?> Get(int id) => Task.FromResult(_getData().FirstOrDefault(c => c.Id == id));
    public Task<Course?> Get(Expression<Func<Course, bool>> action) => Task.FromResult(_getData().AsQueryable().FirstOrDefault(action));
    public Task<List<Course>> GetAll() => Task.FromResult(_getData());
    public Task<List<Course>> GetAll(Expression<Func<Course, bool>> action) => Task.FromResult(_getData().AsQueryable().Where(action).ToList());
    public Task SaveManually() => Task.CompletedTask;
    public Task<List<Course>> ToListAsync(IQueryable<Course> query) => Task.FromResult(query.ToList());
    public Task<Course?> FirstOrDefaultAsync(IQueryable<Course> query) => Task.FromResult(query.FirstOrDefault());
}

/// <summary>
/// In-memory fake repository for Teacher testing
/// </summary>
public class InMemoryTeacherRepository : IRepository<Teacher>
{
    private readonly Func<List<Teacher>> _getData;

    public InMemoryTeacherRepository(Func<List<Teacher>> getData)
    {
        _getData = getData;
    }

    public IQueryable<Teacher> Include<TProperty>(Expression<Func<Teacher, TProperty>> navigationProperty)
    {
        return _getData().AsQueryable();
    }

    public IQueryable<Teacher> Find(Expression<Func<Teacher, bool>> predicate)
    {
        return _getData().AsQueryable().Where(predicate);
    }

    public Type ElementType => _getData().AsQueryable().ElementType;
    public Expression Expression => _getData().AsQueryable().Expression;
    public IQueryProvider Provider => _getData().AsQueryable().Provider;
    public System.Collections.IEnumerator GetEnumerator() => _getData().GetEnumerator();
    IEnumerator<Teacher> IEnumerable<Teacher>.GetEnumerator() => _getData().GetEnumerator();

    public Task<Teacher> CreateAndCommit(Teacher entity) => Task.FromResult(entity);
    public void Create(List<Teacher> entity) => throw new NotImplementedException();
    public void Delete(int id) => throw new NotImplementedException();
    public Task<Teacher> UpdateAndCommit(Teacher entity) => Task.FromResult(entity);
    public void Update(Teacher entity) => throw new NotImplementedException();
    public Task<Teacher> DeleteAndCommit(int id) => throw new NotImplementedException();
    public Task<Teacher?> Get(int id) => Task.FromResult(_getData().FirstOrDefault(t => t.Id == id));
    public Task<Teacher?> Get(Expression<Func<Teacher, bool>> action) => Task.FromResult(_getData().AsQueryable().FirstOrDefault(action));
    public Task<List<Teacher>> GetAll() => Task.FromResult(_getData());
    public Task<List<Teacher>> GetAll(Expression<Func<Teacher, bool>> action) => Task.FromResult(_getData().AsQueryable().Where(action).ToList());
    public Task SaveManually() => Task.CompletedTask;
    public Task<List<Teacher>> ToListAsync(IQueryable<Teacher> query) => Task.FromResult(query.ToList());
    public Task<Teacher?> FirstOrDefaultAsync(IQueryable<Teacher> query) => Task.FromResult(query.FirstOrDefault());
}

/// <summary>
/// In-memory fake repository for Material testing
/// </summary>
public class InMemoryMaterialRepository : IRepository<Material>
{
    private readonly Func<List<Material>> _getData;

    public InMemoryMaterialRepository(Func<List<Material>> getData)
    {
        _getData = getData;
    }

    public IQueryable<Material> Include<TProperty>(Expression<Func<Material, TProperty>> navigationProperty)
    {
        return _getData().AsQueryable();
    }

    public IQueryable<Material> Find(Expression<Func<Material, bool>> predicate)
    {
        return _getData().AsQueryable().Where(predicate);
    }

    public Type ElementType => _getData().AsQueryable().ElementType;
    public Expression Expression => _getData().AsQueryable().Expression;
    public IQueryProvider Provider => _getData().AsQueryable().Provider;
    public System.Collections.IEnumerator GetEnumerator() => _getData().GetEnumerator();
    IEnumerator<Material> IEnumerable<Material>.GetEnumerator() => _getData().GetEnumerator();

    public Task<Material> CreateAndCommit(Material entity) => Task.FromResult(entity);
    public void Create(List<Material> entity) => throw new NotImplementedException();
    public void Delete(int id) => throw new NotImplementedException();
    public Task<Material> UpdateAndCommit(Material entity) => Task.FromResult(entity);
    public void Update(Material entity) => throw new NotImplementedException();
    public Task<Material> DeleteAndCommit(int id) => throw new NotImplementedException();
    public Task<Material?> Get(int id) => Task.FromResult(_getData().FirstOrDefault(m => m.Id == id));
    public Task<Material?> Get(Expression<Func<Material, bool>> action) => Task.FromResult(_getData().AsQueryable().FirstOrDefault(action));
    public Task<List<Material>> GetAll() => Task.FromResult(_getData());
    public Task<List<Material>> GetAll(Expression<Func<Material, bool>> action) => Task.FromResult(_getData().AsQueryable().Where(action).ToList());
    public Task SaveManually() => Task.CompletedTask;
    public Task<List<Material>> ToListAsync(IQueryable<Material> query) => Task.FromResult(query.ToList());
    public Task<Material?> FirstOrDefaultAsync(IQueryable<Material> query) => Task.FromResult(query.FirstOrDefault());
}
