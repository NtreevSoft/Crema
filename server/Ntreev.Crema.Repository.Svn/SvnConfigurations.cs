using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    [Export(typeof(IConfigurationPropertyProvider))]
    class SvnConfigurations : IConfigurationPropertyProvider
    {
        public SvnConfigurations()
        {

        }

        [ConfigurationProperty]
        [DefaultValue(null)]
        public string ExecutablePath
        {
            get => SvnCommand.ExecutablePath;
            set => SvnCommand.ExecutablePath = value;
        }

        [ConfigurationProperty]
        [DefaultValue(0)]
        public int MaxLogCount
        {
            get => SvnLogInfo.MaxCount;
            set => SvnLogInfo.MaxCount = value;
        }

        public string Name => "svn";
    }
}
