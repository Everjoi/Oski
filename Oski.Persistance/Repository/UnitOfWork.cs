using Microsoft.EntityFrameworkCore;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Common;
using Oski.Domain.Common.Interfaces;
using Oski.Persistance.Contexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Persistance.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDataContext _dbContext;
        private Hashtable _repositories;
        private bool disposed;

        public UnitOfWork(AppDataContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IGenericRepository<T> Repository<T>() where T :class, IEntity
        {
            if(_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if(!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)),_dbContext);

                _repositories.Add(type,repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public Task Rollback()
        {
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }

        public async Task<int> Save(Guid userId,CancellationToken cancellationToken)
        {
            foreach(var entry in _dbContext.ChangeTracker.Entries<IAuditableEntity>())
            {
                switch(entry.State)
                {
                    case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    break;

                    case EntityState.Modified:
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
                }
            }

            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<int> SaveAndRemoveCache(CancellationToken cancellationToken,params string[] cacheKeys)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposed)
            {
                if(disposing)
                {
                    _dbContext.Dispose();
                }
            }
            disposed = true;
        }
    }
}
