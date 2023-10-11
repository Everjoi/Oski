using AutoMapper;
using MediatR;
using Oski.Application.Features.Users.Comands.Events;
using Oski.Application.Interfaces.Repositories;
using Oski.Application.Mappings;
using Oski.Domain.Entities;
using Oski.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Users.Comands
{
    public record DeleteUserCommand : IRequest, IMapFrom<User>
    {
        public Guid Id { get; set; }

        public DeleteUserCommand()
        {

        }

        public DeleteUserCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteUserCommandHandler:IRequestHandler<DeleteUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(DeleteUserCommand command,CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(command.Id);
            if(user != null)
            {
                await _unitOfWork.Repository<User>().DeleteAsync(user);
                user.AddDomainEvent(new UserDeletedEvent(user));

                await _unitOfWork.Save(user.Id, cancellationToken);

                return;
            }
            else
            {
                throw new NotFoundException(user.Id);
            }
        }
    }
}
