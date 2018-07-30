using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    [Export(typeof(CremaSettings))]
    class CremaSettings
    {
        public LogVerbose Verbose { get; set; } = LogVerbose.Info;
    }
}
