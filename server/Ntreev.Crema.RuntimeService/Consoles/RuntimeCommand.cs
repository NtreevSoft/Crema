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
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.RuntimeService.Consoles
{
    [Export(typeof(IConsoleCommand))]
    class RuntimeCommand : ConsoleCommandMethodBase
    {
        [Import]
        private Lazy<RuntimeService> runtimeService = null;
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        public RuntimeCommand()
            : base("rt")
        {

        }

        [CommandMethod]
        public void Reset(string dataBaseName)
        {
            var dataBaseID = this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[dataBaseName];
                if (dataBase == null)
                    throw new Exception();
                return dataBase.ID;
            });

            var serviceItem = this.RuntimeService.GetServiceItem(dataBaseID);
            serviceItem.Dispatcher.Invoke(() =>
            {
                serviceItem.Reset();
            });
        }

        [CommandMethod]
        public void Info(string dataBaseName)
        {
            var dataBaseID = this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[dataBaseName];
                if (dataBase == null)
                    throw new Exception();
                return dataBase.ID;
            });

            var serviceItem = this.RuntimeService.GetServiceItem(dataBaseID);
            var info = serviceItem.Dispatcher.Invoke(() => serviceItem.DataServiceItemInfo);

            this.Out.WriteLine();
            this.Out.WriteLine($"Revision : {info.Revision}");
            this.Out.WriteLine($"Version  : {info.Version}");
            this.Out.WriteLine($"DateTime : {info.DateTime}");
            this.Out.WriteLine();
        }

        private RuntimeService RuntimeService => this.runtimeService.Value;

        private ICremaHost CremaHost => this.cremaHost.Value;
    }
}
