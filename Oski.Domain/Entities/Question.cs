using Oski.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Domain.Entities
{
    public class Question : BaseAuditableEntity
    {
        public string Text { get; set; } = string.Empty;
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public Guid TestId { get; set; }
        public Test Test { get; set; }
    }
}
