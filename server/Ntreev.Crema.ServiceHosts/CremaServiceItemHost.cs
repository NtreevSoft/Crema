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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ntreev.Crema.ServiceHosts
{
    public abstract class CremaServiceItemHost : ServiceHost
    {
        private readonly string serviceName;
        private readonly ICremaHost cremaHost;
        private readonly ILogService logService;
        private readonly int port;

        protected CremaServiceItemHost(ICremaHost cremaHost, Type serviceType, string address, int port)
            : base(serviceType, new Uri(string.Format(address, port)))
        {
            this.serviceName = serviceType.Name;
            this.cremaHost = cremaHost;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.port = port;
            this.Uri = new Uri(string.Format(address, port));
#if !DEBUG
            this.DefaultInactivityTimeout = new TimeSpan(0, 1, 0);
#else
            this.DefaultInactivityTimeout = new TimeSpan(0, 2, 0);
#endif
        }

        protected CremaServiceItemHost(ICremaHost cremaHost, object serviceInstance, string address, int port)
            : base(serviceInstance, new Uri(string.Format(address, port)))
        {
            this.serviceName = serviceInstance.GetType().Name;
            this.cremaHost = cremaHost;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.port = port;
            this.Uri = new Uri(string.Format(address, port));
#if !DEBUG
            this.DefaultInactivityTimeout = new TimeSpan(0, 1, 0);
#else
            this.DefaultInactivityTimeout = new TimeSpan(0, 2, 0);
#endif
        }

        public ICremaHost CremaHost
        {
            get { return this.cremaHost; }
        }

        public Uri Uri { get; }

        public abstract object CreateInstance(Message message);

        public static Binding CreateNetTcpBinding()
        {
            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
            return binding;
        }

        public static Binding CreateWebHttpBinding()
        {
            var binding = new WebHttpBinding();
            binding.Security.Mode = WebHttpSecurityMode.None;
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
            return binding;
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            this.logService.Debug("{0}[{1}] is started.", this.serviceName, this.port);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            this.logService.Debug("{0}[{1}] is stopped.", this.serviceName, this.port);
        }

        protected TimeSpan DefaultInactivityTimeout
        {
            get;
            set;
        }
    }
}
