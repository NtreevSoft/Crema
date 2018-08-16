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
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Ntreev.Crema.Data;
using System.Globalization;
using System.Resources;

namespace Ntreev.Crema.ConsoleHost
{
    partial class Program
    {
        static Program()
        {
            
        }

        static void Main(string[] args)
        {
            var excecuteName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            if (args.Length == 0)
            {
                Console.WriteLine("Type '{0} help' for usage.", excecuteName.ToLower());
                return;
            }
            try
            {
                using (var application = new CremaApplication())
                {
                    var configs = application.GetService(typeof(ConsoleConfiguration)) as ConsoleConfiguration;
                    var commandContext = application.GetService(typeof(CommandContext)) as CommandContext;
#if DEBUG
                    commandContext.VerifyName = false;
#endif

                    commandContext.Execute(Environment.CommandLine);
                    configs.Write();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }
    }
}
