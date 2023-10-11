using AutoMapper;
using MediatR;
using Oski.Application.Features.Users.Comands.Events;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using Oski.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Users.Comands
{
    public record UpdateUserCommand:IRequest
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }


    internal class UpdateUserCommandHandler:IRequestHandler<UpdateUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(UpdateUserCommand command,CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(command.Id);

            var us = new User();
            
            if(user != null)
            {
                user.FullName = command.FullName;
                user.Email = command.Email;

                await _unitOfWork.Repository<User>().UpdateAsync(user);
                user.AddDomainEvent(new UserUpdatedEvent(user));

                await _unitOfWork.Save(user.Id,cancellationToken);

                return ;
            }
            else
            {
                throw new NotFoundException(user.Id);
            }
        }
    }

}
