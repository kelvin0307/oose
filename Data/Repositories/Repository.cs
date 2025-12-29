using System.Collections;
using System.Linq.Expressions;
using Core.Interfaces.Repositories;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where
            TEntity : class
    {
        protected DbSet<TEntity> DbSet;
        private readonly DataContext _context;
        private IQueryable<TEntity> _queryable => DbSet;
        public Type ElementType => _queryable.ElementType;

        public Expression Expression => _queryable.Expression;

        public IQueryProvider Provider => _queryable.Provider;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        public Repository(DataContext context)
        {
            _context = context;

            DbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> CreateAndCommit(TEntity entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public void Delete(int id)
        {
            var entity = _context.Set<TEntity>().Find(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Create(List<TEntity> entity)
        {
            _context.Set<List<TEntity>>().Add(entity);
        }

        public async Task<TEntity?> Get(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<TEntity?> Get(Expression<Func<TEntity, bool>> action)
        {
            return await DbSet.FirstOrDefaultAsync(action);
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> action)
        {
            return await DbSet.Where(action).ToListAsync();
        }

        public async Task SaveManually()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> UpdateAndCommit(TEntity entity)
        {
            DbSet.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> DeleteAndCommit(int id)
        {
            try
            {
                var objectData = await DbSet.FindAsync(id) ?? throw new Exception($"Object with id:{id} not found.");

                DbSet.Remove(objectData);
                await _context.SaveChangesAsync();
                return objectData;
            }
            catch (Exception)
            {
                //TODO: add logging with message and stacktrace
                throw;
            }
        }
    }
}
