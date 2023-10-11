using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Persistance.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IGenericRepository<User> _repository;

        public UserRepository(IGenericRepository<User> repository)
        {
            _repository = repository;
        }

    }
}
