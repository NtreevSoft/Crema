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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Crema.Services;
using System.ServiceModel.Channels;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;
using Ntreev.Crema.ServiceHosts.Users;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.ServiceHosts.Users
{
    [Export(typeof(CremaServiceItemHost))]
    class UserServiceHost : CremaServiceItemHost
    {
        private const string address = "net.tcp://localhost:{0}/UserService";
        private readonly CremaService cremaService;

        public UserServiceHost(ICremaHost cremaHost, int port, CremaService cremaService)
            : base(cremaHost, typeof(UserService), address, port)
        {
            this.cremaService = cremaService;
            this.AddServiceEndpoint(typeof(IUserService), CreateNetTcpBinding(), string.Empty);
            this.Description.Behaviors.Add(new InstanceProviderBehavior());

#if DEBUG
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                this.Description.Behaviors.Add(new ServiceMetadataBehavior());
                this.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
            }
#endif
        }

        public override object CreateInstance(Message message)
        {
            return new UserService(this.CremaHost, this.cremaService);
        }
    }
}
