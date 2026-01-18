using System.Linq.Expressions;

namespace Core.Interfaces.Repositories
{
    public interface IRepository<TEntity> : IQueryable<TEntity> where
        TEntity : class
    {
        Task<List<TEntity>> GetAll();
        Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> action);
        Task<TEntity?> Get(int id);
        Task<TEntity?> Get(Expression<Func<TEntity, bool>> action);
        Task<TEntity> CreateAndCommit(TEntity entity);
        void Create(List<TEntity> entity);
        void Update(TEntity entity);
        Task<TEntity> UpdateAndCommit(TEntity entity);
        Task<TEntity> DeleteAndCommit(int id);
        void Delete(int id);
        Task SaveManually();
        Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query);
        Task<TEntity?> FirstOrDefaultAsync(IQueryable<TEntity> query);
        IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationProperty);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}
