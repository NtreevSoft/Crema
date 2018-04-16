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
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.WindowsServiceHost
{
    [Export(typeof(ICommand))]
    [Summary("윈도우 서비스 설치/제거를 위한 명령을 제공합니다.")]
    [Description("윈도우 서비스 설치/제거를 위한 명령을 제공합니다. 이 명령을 실행하기 위해서는 관리자 권한이 필요합니다.")]
    class ServiceCommand : CommandMethodBase
    {
        public ServiceCommand()
            : base("service")
        {

        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Port), nameof(ServiceName), nameof(DisplayName), nameof(Comment))]
        public void Install(string path)
        {
            var basePath = new DirectoryInfo(path).FullName;
            this.InstallService(typeof(Program).Assembly.Location, basePath);
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Port), nameof(ServiceName))]
        public void Uninstall(string path)
        {
            var basePath = new DirectoryInfo(path).FullName;
            this.UninstallService(typeof(Program).Assembly.Location, basePath);
        }

        [CommandProperty]
        [DefaultValue(AddressUtility.DefaultPort)]
        public int Port
        {
            get;
            set;
        }

        [CommandProperty("comment", 'c')]
        [DefaultValue("크레마 서비스를 제공합니다.")]
        public string Comment
        {
            get;
            set;
        }

        [CommandProperty("name")]
        [DefaultValue("CremaService")]
        public string ServiceName
        {
            get;
            set;
        }

        [CommandProperty("display-name")]
        [DefaultValue("Crema Service")]
        public string DisplayName
        {
            get;
            set;
        }

        public override bool IsEnabled => Environment.OSVersion.Platform == PlatformID.Win32NT;

        private void InstallService(string exeFilename, string basePath)
        {
            var pathArg = string.Format("/PATH={0}", basePath);
            var portArg = string.Format("/PORT={0}", this.Port);
            var displayNameArg = string.Format("/DisplayName={0}", this.DisplayName);
            var serviceNameArg = string.Format("/ServiceName={0}", this.ServiceName);
            var comment = string.Format("/Comment={0}", this.Comment);
            var commandLineOptions = new string[] { pathArg, portArg, displayNameArg, serviceNameArg, comment };
            var installer = new AssemblyInstaller(exeFilename, commandLineOptions);

            installer.Install(null);
            installer.Commit(null);
        }

        private void UninstallService(string exeFilename, string basePath)
        {
            var pathArg = string.Format("/PATH={0}", basePath);
            var portArg = string.Format("/PORT={0}", this.Port);
            var displayNameArg = string.Format("/DisplayName={0}", this.DisplayName);
            var serviceNameArg = string.Format("/ServiceName={0}", this.ServiceName);
            var commandLineOptions = new string[] { pathArg, portArg, displayNameArg, serviceNameArg };
            var installer = new AssemblyInstaller(exeFilename, commandLineOptions)
            {
                UseNewContext = true
            };
            installer.Uninstall(null);
        }
    }
}
