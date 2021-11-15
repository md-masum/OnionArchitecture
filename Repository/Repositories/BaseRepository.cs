using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entity;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Repository.Context;

namespace Repository.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            _context.Add(entity);
            return await SaveChangeAsync();
        }

        public virtual async Task<bool> AddListAsync(IList<TEntity> entity)
        {
            _context.AddRange(entity);
            return await SaveChangeAsync();
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await SaveChangeAsync();
        }

        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            _context.Remove(entity);
            return await SaveChangeAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await GetAsQueryable().FirstOrDefaultAsync(x=>x.Id == id);
        }

        public virtual async Task<IList<TEntity>> GetAllAsync()
        {
            return await GetAsQueryable().ToListAsync();
        }
        
        public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAsQueryable().Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAsQueryable().Where(predicate).FirstOrDefaultAsync();
        }

        public virtual IQueryable<TEntity> GetAsQueryable()
        {
            return _context
                .Set<TEntity>().AsQueryable();
        }

        public virtual async Task<bool> UpdateRangeAsync(IList<TEntity> entity)
        {
            _dbSet.UpdateRange(entity);
            return await SaveChangeAsync();
        }

        public async Task<bool> RemoveRangeAsync(IList<TEntity> entity)
        {
            _context.Set<TEntity>().RemoveRange(entity);
            return await SaveChangeAsync();
        }

        private async Task<bool> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
