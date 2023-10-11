using Oski.Domain.Common;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Tests.Comands.Events
{
    public class TestDeletedEvent : BaseEvent
    {
        public Test Test { get; }

        public TestDeletedEvent(Test test)
        {
            Test = test;
        }
    }
}
