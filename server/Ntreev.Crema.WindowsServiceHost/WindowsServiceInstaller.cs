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
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.WindowsServiceHost
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
        private ServiceInstaller serviceInstaller = new ServiceInstaller();

        public WindowsServiceInstaller()
        {
            this.serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Username = null;
            this.serviceProcessInstaller.Password = null;

            this.serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(this.serviceProcessInstaller);
            this.Installers.Add(this.serviceInstaller);
        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            var path = this.Context.Parameters["PATH"];
            var port = this.Context.Parameters["PORT"];
            var filename = this.Context.Parameters["assemblypath"];

            this.Context.Parameters["assemblypath"] = $"{filename.WrapQuot()} {path.WrapQuot()} {port}";

            this.serviceInstaller.Description = this.Context.Parameters["Comment"];
            this.serviceInstaller.DisplayName = this.Context.Parameters["DisplayName"];
            this.serviceInstaller.ServiceName = this.Context.Parameters["ServiceName"];
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        protected override void OnCommitted(System.Collections.IDictionary savedState)
        {
            CremaLog.Debug(serviceInstaller.ServiceName);
            CremaLog.Debug("commit");
            base.OnCommitted(savedState);
            CremaLog.Debug("committed");
            using (ServiceController controller = new ServiceController(serviceInstaller.ServiceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        protected override void OnBeforeUninstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            var path = this.Context.Parameters["PATH"];
            var port = this.Context.Parameters["PORT"];
            var filename = this.Context.Parameters["assemblypath"];

            this.Context.Parameters["assemblypath"] = $"{filename.WrapQuot()} {path.WrapQuot()} {port}";

            this.serviceInstaller.Description = this.Context.Parameters["Comment"];
            this.serviceInstaller.DisplayName = this.Context.Parameters["DisplayName"];
            this.serviceInstaller.ServiceName = this.Context.Parameters["ServiceName"];
        }
    }
}
