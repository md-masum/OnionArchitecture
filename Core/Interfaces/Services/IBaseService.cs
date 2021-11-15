using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entity;

namespace Core.Interfaces.Services
{
    public interface IBaseService<TEntity, TDto> where TEntity : BaseEntity
    {
        Task<TDto> AddAsync(TDto entity);
        Task<TDto> UpdateAsync(TDto entity);
        Task<bool> RemoveAsync(int id);
        Task<TDto> GetByIdAsync(int id);
        Task<IList<TDto>> GetAllAsync();
        Task<IList<TDto>> GetTop(int number);
        Task<IList<TDto>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TDto> GetAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
