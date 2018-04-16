using Ntreev.Crema.Commands;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ApplicationHost
{
    [Export(typeof(IConfigurationProperties))]
    public class ConfigurationProperties : IConfigurationProperties
    {
        private readonly ICremaHost cremaHost;
        private readonly ConfigurationPropertyDescriptorCollection properties = new ConfigurationPropertyDescriptorCollection();

        [ImportingConstructor]
        public ConfigurationProperties(IAppConfiguration configs, ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closed += CremaHost_Closed;
            //foreach (var item in configs.Properties)
            //{
            //    this.properties.Add(item);
            //}
        }

        public ConfigurationPropertyDescriptorCollection Properties => this.properties;

        private void CremaHost_Closed(object sender, EventArgs e)
        {

        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {

        }
    }
}
