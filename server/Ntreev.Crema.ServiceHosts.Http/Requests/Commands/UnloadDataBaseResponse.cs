using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceHosts.Http.Requests.Commands
{
    public enum UnloadDataBaseStatus
    {
        Unloaded,
        AlreadyUnloaded
    }

    public class UnloadDataBaseResponse
    {
        public UnloadDataBaseStatus Status { get; set; }
    }
}
