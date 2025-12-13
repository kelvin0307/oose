namespace Core.Interfaces.Repositories
{
    public interface IRepository<TEntity> : IQueryable<TEntity> where
        TEntity : class
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity> Get(int id);
        Task<TEntity> CreateAndCommit(TEntity entity);
        void Create(List<TEntity> entity);
        void Update(TEntity entity);
        Task<TEntity> UpdateAndCommit(TEntity entity);
        Task<TEntity> DeleteAndCommit(int id);
        void Delete(int id);
        Task SaveManually();
    }
}
