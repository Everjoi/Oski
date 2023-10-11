using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Interfaces
{
    public interface IAuthenticationService
    {
        string Authenticate(string email,string password);
        bool Register(string name, string email,string password);
    }
}
