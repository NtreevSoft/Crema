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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using System.ServiceModel.Description;
using Ntreev.Crema.ServiceModel;
using System.ServiceModel;
using Ntreev.Crema.ServiceHosts.Http;

namespace Ntreev.Crema.ServiceHosts
{
    [Export(typeof(CremaServiceItemHost))]
    class HttpDescriptorServiceHost : CremaServiceItemHost
    {
        private static string address = "http://localhost:{0}/DescriptorService";
        private readonly CremaService service;

        public HttpDescriptorServiceHost(ICremaHost cremaHost, int port, CremaService service)
            : base(cremaHost, typeof(HttpDescriptorService), address, port)
        {
            var binding = CremaServiceItemHost.CreateWebHttpBinding();

            this.service = service;
            var endpoint = this.AddServiceEndpoint(typeof(IHttpDescriptorService), binding, string.Empty);
            endpoint.Behaviors.Add(new WebHttpBehavior());
            this.Description.Behaviors.Add(new InstanceProviderBehavior());

#if DEBUG
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                this.Description.Behaviors.Add(new ServiceMetadataBehavior());
                this.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            }
#endif
        }

        public override object CreateInstance(Message message)
        {
            return new HttpDescriptorService(this.service, this.CremaHost);
        }
    }
}