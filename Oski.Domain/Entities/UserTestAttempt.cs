using Oski.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Domain.Entities
{
    public class UserTestAttempt : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid TestId { get; set; }
        public Test Test { get; set; }
        public int Score { get; set; }
        public DateTime CompletionDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
