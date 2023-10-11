using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Oski.Application.Features.Tests.Queries.DTOsQueries;
using Oski.Application.Features.Users.Queries.DTOsQueries;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Tests.Queries
{
 

    public record GetAllTestsQuery : IRequest<List<GetAllTestsDto>>;

    public class GetAllTestsQueryHandler : IRequestHandler<GetAllTestsQuery,List<GetAllTestsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllTestsQueryHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetAllTestsDto>> Handle(GetAllTestsQuery query,CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Test>().Entities
                   .ProjectTo<GetAllTestsDto>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);   
        }
    }

}
