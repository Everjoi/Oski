using AutoMapper;
using MediatR;
using Oski.Application.Features.Users.Comands.Events;
using Oski.Application.Interfaces.Repositories;
using Oski.Application.Mappings;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Users.Comands
{
    public record CreateUserCommand: IMapFrom<User>, IRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(CreateUserCommand command,CancellationToken cancellationToken)
        {

            var user = new User()
            {
                FullName = command.FullName,
                Email = command.Email,
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            user.AddDomainEvent(new UserCreatedEvent(user));
            await _unitOfWork.Save(user.Id,cancellationToken);

            return;
        }

        
    }
}
