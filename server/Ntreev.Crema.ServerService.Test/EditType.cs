using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServerService.Test
{
    [Flags]
    public enum EditType
    {
        None = 0,

        Create = 1,

        Change = 2,

        Delete = 4,

        All = -1,
    }
}
