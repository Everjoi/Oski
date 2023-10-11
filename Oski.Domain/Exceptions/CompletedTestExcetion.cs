using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Domain.Exceptions
{
    public class CompletedTestExcetion : Exception
    {

        public CompletedTestExcetion(Guid userId) : base($"The user {userId} has already completed this test.")
        {
                
        }
    }
}
