using Oski.Domain.Common;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Features.Users.Comands.Events
{
    internal class UserDeletedEvent : BaseEvent
    {
        public User User { get; }

        public UserDeletedEvent(User user)
        {
            User = user;
        }
    }
}
