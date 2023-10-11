using AutoMapper;
using MediatR;
using Oski.Application.Features.Users.Comands.Events;
using Oski.Application.Features.Users.Comands;
using Oski.Application.Interfaces.Repositories;
using Oski.Application.Mappings;
using Oski.Domain.Entities;
using Oski.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oski.Application.Features.Tests.Comands.Events;

namespace Oski.Application.Features.Tests.Comands
{
    public record DeleteTestCommand : IRequest, IMapFrom<Test>
    {
        public Guid Id { get; set; }

        public DeleteTestCommand()
        {

        }

        public DeleteTestCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteTestCommandHandler : IRequestHandler<DeleteTestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteTestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(DeleteTestCommand command, CancellationToken cancellationToken)
        {
            var test = await _unitOfWork.Repository<Test>().GetByIdAsync(command.Id);
            if (test != null)
            {
                await _unitOfWork.Repository<Test>().DeleteAsync(test);
                test.AddDomainEvent(new TestDeletedEvent(test));

                await _unitOfWork.Save(test.Id, cancellationToken);

                return;
            }
            else
            {
                throw new NotFoundException(test.Id);
            }
        }
    }



}
