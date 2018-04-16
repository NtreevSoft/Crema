using Ntreev.Crema.Commands;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ConsoleHost.Commands
{
    [Export(typeof(ICommand))]
    class ConsoleDataBaseCommand : DataBaseCommandBase
    {
        [Import]
        private Lazy<ConsoleCommandContext> commandContext = null;
        private readonly ICremaHost cremaHost;
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public ConsoleDataBaseCommand(ICremaHost cremaHost)
            : base(cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [CommandMethod("close")]
        public void Close()
        {
            var commandContext = this.commandContext.Value;
            if (commandContext.DataBaseName == string.Empty)
                throw new Exception("데이터 베이스가 열려있지 않습니다.");
            this.cremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.cremaHost.DataBases[commandContext.DataBaseName];
                dataBase.Leave(this.authenticator);
                commandContext.DataBaseName = string.Empty;
            });
        }
    }
}
