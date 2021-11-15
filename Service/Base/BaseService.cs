using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto;
using Core.Entity;
using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Service.Base
{
    public class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto> where TEntity : BaseEntity where TDto:BaseDto
    {
        public readonly IBaseRepository<TEntity> BaseRepository;
        private readonly IMapper _mapper;

        public BaseService(IBaseRepository<TEntity> baseRepository, IMapper mapper)
        {
            BaseRepository = baseRepository;
            _mapper = mapper;
        }

        public virtual async Task<TDto> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await BaseRepository.AddAsync(entity);
            return _mapper.Map<TDto>(entity);
        }
        
        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            if (dto == null) throw new CustomException("No data provided");
            var existing = await BaseRepository.GetByIdAsync(dto.Id);
            if (existing == null) throw new NotFoundException("Data not found");
            existing.UpdateObject(dto);
            await BaseRepository.UpdateAsync(existing);
            return _mapper.Map<TDto>(existing);
        }
        
        public virtual async Task<bool> RemoveAsync(int id)
        {
            var entity = await BaseRepository.GetByIdAsync(id);
            if (entity is null)
            {
                throw new NotFoundException();
            }
            return await BaseRepository.RemoveAsync(entity);
        }
        
        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            var data = await BaseRepository.GetByIdAsync(id);
            if (data is null)
            {
                throw new NotFoundException();
            }
            return _mapper.Map<TDto>(data);
        }
        
        public virtual async Task<IList<TDto>> GetAllAsync()
        {
            var data = await BaseRepository.GetAllAsync();
            return _mapper.Map<IList<TDto>>(data);
        }

        public async Task<IList<TDto>> GetTop(int number)
        {
            var data = await BaseRepository.GetAsQueryable().Take(number).ToListAsync();
            return _mapper.Map<IList<TDto>>(data);
        }

        public async Task<IList<TDto>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var data = await BaseRepository.GetAllAsync(predicate);
            return _mapper.Map<IList<TDto>>(data);
        }

        public async Task<TDto> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var data = await BaseRepository.GetAsync(predicate);
            return _mapper.Map<TDto>(data);
        }
    }
}
