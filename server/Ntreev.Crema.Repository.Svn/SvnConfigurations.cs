using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    [Export(typeof(IConfigurationPropertyProvider))]
    class SvnConfigurations : IConfigurationPropertyProvider
    {
        private string executablePath;

        public SvnConfigurations()
        {
            SvnCommand.ExecutablePath = this.ExecutablePath;
        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        public string ExecutablePath
        {
            get => this.executablePath ?? "svn";
            set
            {
                this.executablePath = value;
                SvnCommand.ExecutablePath = this.ExecutablePath;
            }
        }

        public string Name => "svn";
    }
}
