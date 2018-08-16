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

using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Commands.Consoles.Properties;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [Export(typeof(IConfigurationPropertyProvider))]
    [ResourceDescription("Resources", IsShared = true)]
    class LoggerCommand : ConsoleCommandMethodBase, IConsoleCommand, IConfigurationPropertyProvider
    {
        [ImportMany]
        private IEnumerable<Lazy<ILogService>> logServices = null;
        [Import]
        private Lazy<ICremaHost> cremaHost = null;
        private string viewerFileName;
        private string viewerArguments;

        public LoggerCommand()
        {

        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            if (methodDescriptor.DescriptorName == nameof(Open))
            {
                if (memberDescriptor.DescriptorName == "logName")
                {
                    return this.GetLogList().Select(item => item.Name).ToArray();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Verbose))
            {
                if (memberDescriptor.DescriptorName == "logName")
                {
                    return this.GetLogList().Select(item => item.Name).ToArray();
                }
                else if (memberDescriptor.DescriptorName == "verbose")
                {
                    return Enum.GetNames(typeof(LogVerbose));
                }
            }
            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [CommandMethod]
        public void Verbose(string logName, string verbose = null)
        {
            var logService = this.GetLog(logName);
            if (verbose == null)
            {
                this.CommandContext.WriteLine(logService.Verbose.ToString().ToLower());
            }
            else
            {
                if (Enum.TryParse(verbose, true, out LogVerbose value) == true)
                {
                    logService.Verbose = value;
                }
                else
                {
                    throw new ArgumentException($"{verbose} is invalide verbose type.");
                }
            }
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(DetailProperties))]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void List()
        {
            if (DetailProperties.IsDetail == true)
            {
                foreach (var item in this.GetLogList())
                {
                    var props = new Dictionary<string, object>()
                    {
                        { nameof(ILogService.Name), item.Name},
                        { nameof(ILogService.Verbose), item.Verbose},
                        { nameof(ILogService.FileName),item.FileName}
                    };
                    this.CommandContext.WriteObject(props, FormatProperties.Format);
                    this.CommandContext.WriteLine();
                }
            }
            else
            {
                var names = this.GetLogList().Select(item => item.Name).ToArray();
                this.CommandContext.WriteList(names);
            }
        }

        [CommandMethod]
        public void Open(string logName)
        {
            var logService = this.GetLog(logName);
            if (logService.FileName == string.Empty)
                throw new InvalidOperationException("'{0}' does not provide file service");

            var startInfo = new ProcessStartInfo();
            if (ViewerFileName != string.Empty)
            {
                startInfo.UseShellExecute = false;
                startInfo.FileName = ViewerFileName;
                startInfo.Arguments = $"{logService.FileName.WrapQuot()} {ViewerArguments}";
            }
            else
            {
                startInfo.UseShellExecute = false;
                startInfo.FileName = "notepad";
                startInfo.Arguments = logService.FileName.WrapQuot();
            }

            Process.Start(startInfo);
        }

        [ConfigurationProperty("viewerFileName")]
        [DefaultValue("")]
        public string ViewerFileName
        {
            get => this.viewerFileName ?? string.Empty;
            set => this.viewerFileName = value;
        }

        [ConfigurationProperty("viewerArguments")]
        [DefaultValue("")]
        public string ViewerArguments
        {
            get => this.viewerArguments ?? string.Empty;
            set => this.viewerArguments = value;
        }

        protected override bool IsMethodEnabled(CommandMethodDescriptor descriptor)
        {
            if (descriptor.DescriptorName == nameof(Open) && Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                return false;
            }

            return base.IsMethodEnabled(descriptor);
        }

        private IEnumerable<ILogService> GetLogList()
        {
            return this.logServices.Select(item => item.Value).Where(item => item.IsEnabled);
        }

        private ILogService GetLog(string logName)
        {
            var logService = this.GetLogList().FirstOrDefault(item => item.Name == logName);
            if (logService == null)
                throw new ItemNotFoundException(logName);
            return logService;
        }

        private ICremaHost CremaHost => this.cremaHost.Value;
    }
}
