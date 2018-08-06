using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    [Export(typeof(IConfigurationPropertyProvider))]
    class GitConfigurations : IConfigurationPropertyProvider
    {
        public GitConfigurations()
        {

        }

        [ConfigurationProperty]
        [DefaultValue(null)]
        public string ExecutablePath
        {
            get => GitCommand.ExecutablePath;
            set => GitCommand.ExecutablePath = value;
        }

        [ConfigurationProperty]
        [DefaultValue(0)]
        public int MaxLogCount
        {
            get => GitLogInfo.MaxCount;
            set => GitLogInfo.MaxCount = value;
        }

        public string Name => "git";
    }
}
