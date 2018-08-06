using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    [Export(typeof(IConfigurationPropertyProvider))]
    class GitConfigurations : IConfigurationPropertyProvider
    {
        private string executablePath;

        public GitConfigurations()
        {
            GitCommand.ExecutablePath = this.ExecutablePath;
        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        public string ExecutablePath
        {
            get => this.executablePath ?? "git";
            set
            {
                this.executablePath = value;
                GitCommand.ExecutablePath = this.ExecutablePath;
            }
        }

        public string Name => "git";
    }
}
