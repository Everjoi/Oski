using AutoMapper;
using MediatR;
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
    public record GetUserByIdQuery : IRequest
    {
        public Guid Id { get; set; }

        public GetUserByIdQuery()
        {

        }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    internal class GetUserByIdQueryHandler:IRequestHandler<GetUserByIdQuery>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(GetUserByIdQuery query,CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Repository<User>().GetByIdAsync(query.Id);
            var player = _mapper.Map<GetUserByIdDto>(entity);
            return ;
        }
    }
}
