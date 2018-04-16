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
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Crema.WindowsServiceHost.Properties;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Crema.ServiceModel;
using System.Threading.Tasks;
using System.Threading;

namespace Ntreev.Crema.WindowsServiceHost
{
    public partial class WindowCremaService : ServiceBase
    {
        private CremaService cremaService;
        private ICremaHost cremaHost;

        public WindowCremaService()
        {
            this.ServiceName = "Crema Service";
            this.EventLog.Log = "Crema";

            this.CanPauseAndContinue = false;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            var baseArgs = Environment.GetCommandLineArgs();
            var path = baseArgs[1];
            var port = int.Parse(baseArgs[2]);

            this.cremaService = new CremaService()
            {
                BasePath = path,
                Port = port,
            };

            CremaLog.Debug("service base path : {0}", path);
            CremaLog.Debug("service port : {0}", port);

            try
            {
                CremaLog.Debug(args.Length);
                if (args.Any() == true)
                {
                    var settings = new Settings()
                    {
                        BasePath = this.cremaService.BasePath,
                        Port = this.cremaService.Port,
                        RepositoryModule = this.cremaService.RepositoryModule,
                        RepositoryName = this.cremaService.RepositoryName,
                    };
                    var parser = new CommandLineParser("setting", settings);
                    parser.Parse(string.Join(" ", "setting", string.Join(" ", args)));

                    this.cremaService.BasePath = settings.BasePath;
                    this.cremaService.Port = settings.Port;
                    this.cremaService.RepositoryModule = settings.RepositoryModule;
                    this.cremaService.RepositoryName = settings.RepositoryName;

                    CremaLog.Debug("=========================================================");
                    CremaLog.Debug("new settings");
                    CremaLog.Debug("service base path : {0}", settings.BasePath);
                    CremaLog.Debug("service port : {0}", settings.Port);
                    CremaLog.Debug("service repo name : {0}", settings.RepositoryName);
                    CremaLog.Debug("service repo module : {0}", settings.RepositoryModule);
                    CremaLog.Debug("=========================================================");
                }
            }
            catch (Exception e)
            {
                CremaLog.Error(e);
                throw e;
            }

            CremaLog.Debug("service open");
            this.cremaService.Open();
            this.cremaHost = this.cremaService.CremaHost;
            this.cremaHost.Closed += CremaHost_Closed;
            CremaLog.Debug("service opened.");
        }

        protected override void OnStop()
        {
            base.OnStop();
            CremaLog.Debug("service close");
            this.cremaService.Close();
            CremaLog.Debug("service closed.");
        }

        private void CremaHost_Closed(object sender, ClosedEventArgs e)
        {
            CremaLog.Debug(nameof(CremaHost_Closed) + " " + e.Reason);
            if (e.Reason == CloseReason.Shutdown)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    this.Stop();
                });
            }
        }
    }
}
