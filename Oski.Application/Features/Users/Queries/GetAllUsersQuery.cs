using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Oski.Application.Features.Users.Queries.DTOsQueries;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Users.Queries
{
    public record GetAllUsersQuery:IRequest;

    internal class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(GetAllUsersQuery query,CancellationToken cancellationToken)
        {
            var players = await _unitOfWork.Repository<User>().Entities
                   .ProjectTo<GetAllUsersDto>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);
            return ;
        }
    }
}
