using Oski.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Domain.Entities
{
    public class UserAnswer : IEntity
    {
        public Guid AttemptId { get; set; }
        public Guid AnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public Guid Id { get ; set; }
    }

}
