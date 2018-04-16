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
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.ConsoleHost.Commands;
using Ntreev.Crema.ConsoleHost.Properties;
using System.Resources;
using System.Reflection;
using System.IO;

namespace Ntreev.Crema.ConsoleHost
{
    partial class Program
    {
        static Program()
        {
            RegisterLicense($"{typeof(Program).Namespace}.Licenses.Newtonsoft.Json.Schema.license", item => Newtonsoft.Json.Schema.License.RegisterLicense(item));

            void RegisterLicense(string licenseName, Action<string> action)
            {
                try
                {
                    using (var stream = typeof(Program).Assembly.GetManifestResourceStream(licenseName))
                    using (var reader = new StreamReader(stream))
                    {
                        var license = reader.ReadToEnd();
                        if (license == string.Empty)
                            CremaLog.Warn($"license does not registered : {licenseName}");
                        else
                            action(license);
                    }
                }
                catch (Exception e)
                {
                    CremaLog.Error(new Exception($"register license failed : {licenseName}", e));
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                using (var application = new CremaBootstrapper())
                {
                    var configs = application.GetService(typeof(ConsoleConfiguration)) as ConsoleConfiguration;
                    var commandContext = application.GetService(typeof(CommandContext)) as CommandContext;
#if DEBUG
                    commandContext.VerifyName = false;
#endif
                    commandContext.Execute(Environment.CommandLine);
                    configs.Commit();
                }
            }
            catch (TargetInvocationException e)
            {
                Console.Error.WriteLine(e.InnerException.ToString());
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Environment.Exit(1);
            }
        }
    }
}
