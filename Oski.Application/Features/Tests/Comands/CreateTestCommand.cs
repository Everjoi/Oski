using AutoMapper;
using MediatR;
using Oski.Application.Features.Tests.Comands.Events;
using Oski.Application.Features.Users.Comands;
using Oski.Application.Features.Users.Comands.Events;
using Oski.Application.Interfaces.Repositories;
using Oski.Application.Mappings;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Tests.Comands
{
    public record CreateTestCommand:IMapFrom<Test>, IRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxScore  { get; set; }
        public ICollection<Question> Questions { get; set; }
    }


    public class CreateTestCommandHendler :IRequestHandler<CreateTestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTestCommandHendler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(CreateTestCommand command,CancellationToken cancellationToken)
        {

            var test = new Test()
            {
                Title = command.Title,
                Description = command.Description,
                MaxScore = command.MaxScore,
                Questions = command.Questions
            };

            await _unitOfWork.Repository<Test>().AddAsync(test);
            test.AddDomainEvent(new TestCreatedEvent(test));
            await _unitOfWork.Save(test.Id,cancellationToken);

            return;
        }
    }

}
