using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entity;

namespace Core.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<bool> AddAsync(TEntity entity);
        Task<bool> AddListAsync(IList<TEntity> entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> RemoveAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(int id);
        Task<IList<TEntity>> GetAllAsync();
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAsQueryable();
        Task<bool> UpdateRangeAsync(IList<TEntity> entity);
        Task<bool> RemoveRangeAsync(IList<TEntity> entity);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
