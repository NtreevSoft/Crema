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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data.Xml;
using Ntreev.Library.Linq;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceHosts.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.ServiceModel;
using System.Diagnostics;
using System.Globalization;
using Ntreev.Library.Extensions;

namespace Ntreev.Crema.ServiceHosts
{
    public class CremaService : CremaBootstrapper, ICremaService
    {
        public const string Namespace = "http://www.ntreev.com";
        private const string cremaString = "Crema";
        private static readonly CremaDispatcher dispatcher;

        private readonly List<ServiceHost> hosts = new List<ServiceHost>();
        private ICremaHost cremaHost;
        private ILogService logService;
        private int port = AddressUtility.DefaultPort;
        private DescriptorService descriptorService;
        private DescriptorServiceHost descriptorServiceHost;
        private List<ServiceInfo> serviceInfos = new List<ServiceInfo>();
        private Guid token;
        private int httpPort;

        static CremaService()
        {
            dispatcher = new CremaDispatcher(typeof(CremaServiceItemHost));
        }

        public CremaService()
        {
            this.MultiThreading = true;
        }

        public void Open()
        {
            this.OnOpening(EventArgs.Empty);

            this.cremaHost = this.GetService(typeof(ICremaHost)) as ICremaHost;
            this.logService = this.cremaHost.GetService(typeof(ILogService)) as ILogService;

            this.token = this.cremaHost.Open();
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closing += CremaHost_Closing;
            CremaService.Dispatcher.Invoke(() =>
            {
                this.descriptorService = new DescriptorService(this, this.cremaHost);
                this.descriptorServiceHost = new DescriptorServiceHost(this.cremaHost, this.descriptorService, this.port);
                this.descriptorServiceHost.Open();
                this.logService.Info(Resources.ServiceStart, nameof(DescriptorServiceHost));
            });
            this.StartWcfServices();
            this.StartHttpServices();

            this.OnOpened(EventArgs.Empty);
        }

        public void Close()
        {
            this.OnClosing(EventArgs.Empty);
            this.StopWcfServices();
            this.StopHttpServices();
            CremaService.Dispatcher.Invoke(() =>
            {
                this.descriptorServiceHost.Close();
                this.descriptorServiceHost = null;
                this.descriptorService = null;
                this.logService.Info(Resources.ServiceStop, nameof(DescriptorServiceHost));
            });
            if (this.cremaHost.IsOpened == true)
            {
                this.cremaHost.Opened -= CremaHost_Opened;
                this.cremaHost.Closing -= CremaHost_Closing;
                this.cremaHost.Close(this.token);
                this.cremaHost.SaveConfigs();
            }
            this.token = Guid.Empty;
            this.OnClosed(EventArgs.Empty);
        }

        public void Restart()
        {
            this.StopWcfServices();
            this.StopHttpServices();
            this.cremaHost.Dispatcher.Invoke(() => this.CremaHost.Close(this.token));
            this.cremaHost.SaveConfigs();
            this.token = this.cremaHost.Dispatcher.Invoke(() => this.CremaHost.Open());
            this.StartWcfServices();
            this.StartHttpServices();
        }

        public int Port
        {
            get { return this.port; }
            set { this.port = value; }
        }



        public bool NoHttpServer { get; set; }

        public int? HttpPort
        {
            get => this.httpPort;
            set => this.httpPort = value ?? AddressUtility.GetDefaultHttpPort(this.port);
        }

        public ICremaHost CremaHost
        {
            get { return this.cremaHost; }
        }

        public ServiceInfo[] ServiceInfos
        {
            get { return this.serviceInfos.ToArray(); }
        }

        public static CremaDispatcher Dispatcher
        {
            get { return dispatcher; }
        }

        public event EventHandler Opening;

        public event EventHandler Opened;

        public event EventHandler Closing;

        public event EventHandler Closed;

        public override IEnumerable<Tuple<System.Type, object>> GetParts()
        {
            foreach (var item in base.GetParts())
            {
                yield return item;
            }
            yield return new Tuple<Type, object>(typeof(CremaService), this);
            yield return new Tuple<Type, object>(typeof(ICremaService), this);
        }

        protected virtual void OnOpening(EventArgs e)
        {
            this.Opening?.Invoke(this, e);
        }

        protected virtual void OnOpened(EventArgs e)
        {
            this.Opened?.Invoke(this, e);
        }

        protected virtual void OnClosing(EventArgs e)
        {
            this.Closing?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
            dispatcher.Dispose();
        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            this.StartWcfServices();
            this.StartHttpServices();
        }

        private void CremaHost_Closing(object sender, EventArgs e)
        {
            this.StopWcfServices();
            this.StopHttpServices();
        }

        private void StartWcfServices()
        {
            var providers = this.GetService(typeof(IEnumerable<IServiceHostProvider>)) as IEnumerable<IServiceHostProvider>;
            var items = providers.TopologicalSort().ToArray();
            var ports = new Dictionary<string, int>();
            var initPort = this.port;
            foreach (var schema in providers.Select(o => o.Schema).Distinct())
            {
                ports.Add(schema, initPort);
                initPort += 100;
            };
            var version = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion);

            CremaService.Dispatcher.Invoke(() =>
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    ports.ToList().ForEach(o => ports[o.Key] += 2);
                }

                foreach (var item in items)
                {
                    var port = ports[item.Schema];
                    var host = item.CreateInstance(port);
                    host.Open();
                    
                    this.hosts.Add(host);
                    this.logService.Info(Resources.ServiceStart_Port, host.GetType().Name, port);
                    this.serviceInfos.Add(new ServiceInfo()
                    {
                        Name = item.Name,
                        Port = port,
#if DEBUG
                        PlatformID = $"DEBUG_{Environment.OSVersion.Platform}",
#else
                        PlatformID = $"{Environment.OSVersion.Platform}",
#endif
                        Version = $"{version}",
                        Culture = $"{CultureInfo.CurrentCulture}"
                    });
                    if (Environment.OSVersion.Platform == PlatformID.Unix)
                        ports[item.Schema]++;
                }
                this.logService.Info(Resources.ServiceStart, cremaString);
            });
        }

        private void StartHttpServices()
        {
            if (this.NoHttpServer) return;
            if (this.HttpPort == null) throw new NullReferenceException(nameof(this.HttpPort));

            var httpHosts = this.GetService<IEnumerable<IHttpServiceHost>>();

            CremaService.Dispatcher.Invoke(() =>
            {
                foreach (var host in httpHosts)
                {
                    host.Open(this.HttpPort.Value);
                    this.logService.Info(Resources.ServiceStart_Port, host.GetType().Name, this.HttpPort);
                }
            });
        }

        private void StopWcfServices()
        {
            CremaService.Dispatcher.Invoke(() =>
            {
                if (this.hosts.Any() == false)
                    return;

                foreach (var item in this.hosts.Reverse<ServiceHost>())
                {
                    item.Close();
                    this.logService.Info(Resources.ServiceStop, item.GetType().Name);
                }

                this.hosts.Clear();
                this.serviceInfos.Clear();
                this.logService.Info(Resources.ServiceStop, cremaString);
            });
        }

        private void StopHttpServices()
        {
            if (this.NoHttpServer) return;
            if (this.HttpPort == null) throw new NullReferenceException(nameof(this.HttpPort));

            var httpHosts = this.GetService<IEnumerable<IHttpServiceHost>>();

            CremaService.Dispatcher.Invoke(() =>
            {
                foreach (var host in httpHosts)
                {
                    host.Close();
                    this.logService.Info(Resources.ServiceStop, cremaString);
                }
            });
        }
    }
}
