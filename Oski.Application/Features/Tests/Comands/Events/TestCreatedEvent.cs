using Oski.Domain.Common;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Tests.Comands.Events
{
    public  class TestCreatedEvent : BaseEvent
    {
        public Test Test { get; }

        public TestCreatedEvent(Test test)
        {
            Test = test;
        }
    }
}
