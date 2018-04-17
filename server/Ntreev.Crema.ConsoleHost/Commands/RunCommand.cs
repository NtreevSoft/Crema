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
using Ntreev.Crema.ConsoleHost.Properties;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.ConsoleHost.Commands.Consoles;
using System.Collections.Generic;

namespace Ntreev.Crema.ConsoleHost.Commands
{
    [Export(typeof(ICommand))]
    [Export]
    [ResourceDescription]
    class RunCommand : CommandBase
    {
        private readonly CremaApplication application;
        private string repositoryModule;

        [ImportingConstructor]
        public RunCommand(CremaApplication application)
            : base("run")
        {
            this.application = application;
        }

        public void Cancel()
        {
            var terminal = this.application.GetService(typeof(ConsoleTerminal)) as ConsoleTerminal;
            terminal.Cancel();
        }

        [CommandProperty("path", IsRequired = true)]
        public string Path
        {
            get;
            set;
        }

        [CommandProperty("port")]
        [DefaultValue(AddressUtility.DefaultPort)]
        public int Port
        {
            get;
            set;
        }

        [CommandProperty("repo-name")]
        public string RepositoryName
        {
            get;
            set;
        }

        [CommandProperty("repo-module")]
        public string RepositoryModule
        {
            get { return this.repositoryModule ?? CremaBootstrapper.DefaultRepositoryModule; }
            set
            {
                this.repositoryModule = value;
            }
        }

        [CommandProperty("prompt", 'p')]
        public bool IsPromptMode
        {
            get;
            set;
        }

        [CommandProperty]
        public bool Verbose
        {
            get; set;
        }

        [CommandProperty]
        public bool NoCache
        {
            get;
            set;
        }

        [CommandProperty]
        public bool OmitRestore
        {
            get;
            set;
        }

        [CommandProperty]
#if DEBUG
        [DefaultValue("en-US")]
#else
        [DefaultValue("")]
#endif
        public string Culture
        {
            get; set;
        }

        [CommandPropertyArray]
        [Description("database list to load")]
        public string[] DataBaseList
        {
            get; set;
        }

#if DEBUG

        [CommandProperty('l', IsExplicit = true)]
        [DefaultValue("admin:admin")]
        public string LoginAuthentication
        {
            get;
            set;
        }

        [CommandProperty("validation", 'v')]
        public bool ValidationMode
        {
            get;
            set;
        }
#endif

        protected override void OnExecute()
        {
            CremaLog.Verbose = this.Verbose ? LogVerbose.Debug : LogVerbose.Info;
            this.application.BasePath = this.Path;
            this.application.RepositoryName = this.RepositoryName;
            this.application.RepositoryModule = this.RepositoryModule;
            this.application.Verbose = this.Verbose ? LogVerbose.Debug : LogVerbose.Info;
            this.application.NoCache = this.NoCache;
            this.application.Culture = this.Culture;
            this.application.DataBaseList = this.DataBaseList;
#if DEBUG
            this.application.ValidationMode = this.ValidationMode;
#endif
            this.application.Port = this.Port;
            this.application.Open();

            var cremaHost = this.application.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Closed += CremaHost_Closed;
            this.Wait();
            Console.WriteLine(Resources.StoppingServer);
            this.application.Close();
            Console.WriteLine(Resources.ServerHasBeenStopped);
        }

        private void CremaHost_Closed(object sender, ClosedEventArgs e)
        {
            if (e.Reason == CloseReason.Shutdown)
            {
                this.Cancel();
            }
        }

        private void Wait()
        {
            if (this.IsPromptMode == true)
            {
                var terminal = this.application.GetService(typeof(ConsoleTerminal)) as ConsoleTerminal;
#if DEBUG
                terminal.Start(this.LoginAuthentication);
#else
                terminal.Start();
#endif
            }
            else
            {
                Console.WriteLine(Resources.ConnectionMessage);
                if (Console.IsInputRedirected == false)
                {
                    while (Console.ReadKey().Key != ConsoleKey.Q) ;
                }
                else
                {
                    while (Console.ReadLine() != "exit") ;
                }
            }
        }
    }
}
