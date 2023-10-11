using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Persistance.Repository
{
    public class TestRepository : ITestRepository
    {
        private readonly IGenericRepository<Test> _repository;

        public TestRepository(IGenericRepository<Test> repository)
        {
            _repository = repository;
        }

    }
}
