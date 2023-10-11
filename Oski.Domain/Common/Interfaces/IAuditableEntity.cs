using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Domain.Common.Interfaces
{
    public interface IAuditableEntity : IEntity
    {
        Guid? CreatedBy { get; set; }
        DateTime? CreatedDate { get; set; }
        Guid? UpdatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
    }
}
