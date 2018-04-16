using Ntreev.Crema.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;

namespace Ntreev.Crema.Client.Commands
{
    [Export(typeof(ClientCommandContext))]
    [Export(typeof(CremaCommandContextBase))]
    public class ClientCommandContext : CremaCommandContextBase
    {
        [ImportingConstructor]
        public ClientCommandContext(ICremaHost cremaHost, [ImportMany] IEnumerable<ICommand> commands) 
            : base(cremaHost, commands)
        {
        }
    }
}
