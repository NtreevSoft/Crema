using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    class SvnCommand : CommandHost
    {
        private const string svn = "svn";

        public SvnCommand(string commandName)
            : base(SvnCommand.ExecutablePath ?? svn, null, commandName)
        {

        }

        public string Run(ILogService logService)
        {
            logService.Debug(this.ToString());
            return this.Run();
        }

        public static string ExecutablePath { get; set; }
    }
}