using Microsoft.EntityFrameworkCore;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Common;
using Oski.Domain.Common.Interfaces;
using Oski.Domain.Exceptions;
using Oski.Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Persistance.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T :  BaseAuditableEntity
    {
        private readonly AppDataContext _dbContext;

        public GenericRepository(AppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            T exist = _dbContext.Set<T>().Find(entity.Id);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var result = await _dbContext.Set<T>().FindAsync(id);
            
            if(result == null)
                throw new NotFoundException();

            return result;
        }
    }
}
