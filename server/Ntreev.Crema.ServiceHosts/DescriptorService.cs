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
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    class DescriptorService : IDescriptorService
    {
        private readonly CremaService service;
        private readonly ICremaHost cremaHost;

        public DescriptorService(CremaService service, ICremaHost cremaHost)
        {
            this.service = service;
            this.cremaHost = cremaHost;
        }

        public ServiceInfo[] GetServiceInfos()
        {
            return this.service.ServiceInfos;
        }

        public DataBaseInfo[] GetDataBaseInfos()
        {
            return this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases.Select(item => item.DataBaseInfo).ToArray());
        }

        public string GetVersion()
        {
            return AppUtility.ProductVersion.ToString();
        }

        public bool IsOnline(string userID, byte[] password)
        {
            var userContext = this.cremaHost.GetService(typeof(IUserContext)) as IUserContext;
            return userContext.Dispatcher.Invoke(() =>
            {
                var text = Encoding.UTF8.GetString(password);
                return userContext.IsOnlineUser(userID, StringUtility.ToSecureString(StringUtility.Decrypt(text, userID)));
            });
        }
    }
}
