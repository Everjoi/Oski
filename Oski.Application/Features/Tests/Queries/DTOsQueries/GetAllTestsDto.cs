using Oski.Application.Mappings;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Tests.Queries.DTOsQueries
{
    public class GetAllTestsDto : IMapFrom<Test>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxScore { get; set; }
    }
}
