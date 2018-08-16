//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Ntreev.Crema.Commands;
using System;
using System.Linq;
using System.ComponentModel.Composition;
using Ntreev.Library;
using System.Collections.Generic;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ConsoleHost
{
    [Export(typeof(IConfigurationProperties))]
    public class ConfigurationProperties : IConfigurationProperties
    {
        private readonly ICremaHost cremaHost;
        private readonly ConfigurationPropertyDescriptorCollection properties;
        private readonly List<ConfigurationPropertyDescriptor> disabledProperties;

        [ImportingConstructor]
        public ConfigurationProperties(IConsoleConfiguration configs, ICremaHost cremaHost, [ImportMany]IEnumerable<IConfigurationPropertyProvider> providers)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closed += CremaHost_Closed;
            this.properties = new ConfigurationPropertyDescriptorCollection(providers);
            this.disabledProperties = new List<ConfigurationPropertyDescriptor>();

            if (this.cremaHost.IsOpened == false)
                this.DetachCremaConfigs();
        }

        public ConfigurationPropertyDescriptorCollection Properties => this.properties;

        private void CremaHost_Closed(object sender, EventArgs e)
        {
            this.DetachCremaConfigs();
        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            this.AttachCremaConfigs();
        }

        private void AttachCremaConfigs()
        {
            foreach(var item in this.disabledProperties)
            {
                this.properties.Add(item);
            }
            this.disabledProperties.Clear();
        }

        private void DetachCremaConfigs()
        {
            foreach (var item in this.properties.ToArray())
            {
                if (item.ScopeType == typeof(ICremaConfiguration))
                {
                    this.properties.Remove(item);
                    this.disabledProperties.Add(item);
                }
            }
        }
    }
}