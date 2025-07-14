using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibraries.UserServices
{
    public interface IUserContextService
    {
        string UserId { get; }
        string Email { get; }
        string Name { get; }
        List<string> Roles { get; }
    }

}
