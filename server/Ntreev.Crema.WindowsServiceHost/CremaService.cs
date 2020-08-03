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
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceHosts;
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

        private static bool IsMonoRuntime => Type.GetType("Mono.Runtime") != null;
        private const int MonoRuntimeCommandLineArgsCount = 3;

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            try
            {
                var environmentCommandLineArgs = Environment.GetCommandLineArgs();
                var baseArgs = IsMonoRuntime
                    ? environmentCommandLineArgs.Skip(environmentCommandLineArgs.Length - MonoRuntimeCommandLineArgsCount - 1).Take(MonoRuntimeCommandLineArgsCount + 1).ToArray()
                    : environmentCommandLineArgs;
                var path = baseArgs[1];
                var port = int.Parse(baseArgs[2]);
                var httpPort = int.Parse(baseArgs[3]);

                this.cremaService = new CremaService()
                {
                    BasePath = path,
                    Port = port,
                    HttpPort = httpPort
                };

                CremaLog.Debug(args.Length);
                if (!IsMonoRuntime && args.Any())
                {
                    var settings = new Settings()
                    {
                        BasePath = this.cremaService.BasePath,
                        Port = this.cremaService.Port,
                        HttpPort = this.cremaService.HttpPort.Value,
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
                    CremaLog.Debug("service http port : {0}", settings.HttpPort);
                    CremaLog.Debug("service repo name : {0}", settings.RepositoryName);
                    CremaLog.Debug("service repo module : {0}", settings.RepositoryModule);
                    CremaLog.Debug("=========================================================");
                }
                else if (args.Any())
                {
                    CremaLog.Debug("service base path : {0}", this.cremaService.BasePath);
                    CremaLog.Debug("service port : {0}", this.cremaService.Port);
                    CremaLog.Debug("service http port : {0}", this.cremaService.HttpPort ?? -1);
                    CremaLog.Debug("service repo name : {0}", this.cremaService.RepositoryName);
                    CremaLog.Debug("service repo module: {0}", this.cremaService.RepositoryModule);
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
            StopService();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            CremaLog.Debug("Computer shutdown.");
            StopService();
        }

        private void StopService()
        {
            CremaLog.Debug("service close");
            this.cremaService.Close();
            CremaLog.Debug("service closed.");

            if (IsMonoRuntime) Environment.Exit(0);
        }

        private void CremaHost_Closed(object sender, ClosedEventArgs e)
        {
            CremaLog.Debug(nameof(CremaHost_Closed) + " " + e.Reason);
            if (e.Reason == CloseReason.Shutdown)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    this.StopService();
                });
            }
        }
    }
}
