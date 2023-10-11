using Oski.Domain.Common;
using Oski.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Interfaces.Repositories
{
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class, IEntity;
        Task<int> Save(Guid userid,CancellationToken cancellationToken);
        Task<int> SaveAndRemoveCache(CancellationToken cancellationToken,params string[] cacheKeys);
        Task Rollback();
    }
}
