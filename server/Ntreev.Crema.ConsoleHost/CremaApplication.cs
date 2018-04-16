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
using Ntreev.Crema.ServiceHosts;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Ntreev.Crema.WindowsServiceHost;
using Ntreev.Library.Linq;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ConsoleHost
{
    class CremaApplication : CremaService
    {
        public CremaApplication()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public override IEnumerable<Tuple<System.Type, object>> GetParts()
        {
            foreach (var item in base.GetParts())
            {
                yield return item;
            }
            yield return new Tuple<Type, object>(typeof(CremaApplication), this);
        }

        public override IEnumerable<Assembly> GetAssemblies()
        {
            return EnumerableUtility.Friends(typeof(WindowCremaService).Assembly, base.GetAssemblies());
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var publishers = this.GetService(typeof(IEnumerable<IExceptionPublisher>)) as IEnumerable<IExceptionPublisher>;
                foreach (var item in publishers)
                {
                    item.Publish(e.ExceptionObject);
                }
            }
            catch (Exception)
            {
            }

            Environment.Exit(-1);
        }
    }
}
