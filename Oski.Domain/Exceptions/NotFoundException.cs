using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Domain.Exceptions
{
    public class NotFoundException : Exception
    {

        public NotFoundException(Guid id) : base($"User {id} not foud")
        {
                
        }
    }
}
