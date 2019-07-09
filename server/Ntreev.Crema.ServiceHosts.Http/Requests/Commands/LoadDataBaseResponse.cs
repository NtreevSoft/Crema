using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceHosts.Http.Requests.Commands
{
    public enum LoadDataBaseStatus
    {
        Loaded,
        AlreadyLoaded
    }

    public class LoadDataBaseResponse
    {
        public LoadDataBaseStatus Status { get; set; }
    }
}
