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

using Ntreev.Crema.ConsoleHost.Commands.Consoles;
using Ntreev.Crema.Javascript;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.ConsoleHost.Commands
{
    [Export(typeof(ICommand))]
    [Export]
    [ResourceDescription]
    class RunCommand : CommandBase, Ntreev.Library.Commands.IUnknownArgument
    {
        private readonly CremaBootstrapper application;
        private readonly ICremaHost cremaHost;
        [Import]
        private Lazy<ScriptContext> scriptContext = null;

        [ImportingConstructor]
        public RunCommand(CremaBootstrapper application, ICremaHost cremaHost)
            : base("run")
        {
            this.application = application;
            this.cremaHost = cremaHost;

        }

        [CommandProperty(IsRequired = true)]
        [CommandPropertyTrigger(nameof(List), false)]
        [DefaultValue("")]
        public string ScriptPath
        {
            get;
            set;
        }

        [CommandProperty("list", 'l')]
        [CommandPropertyTrigger(nameof(ScriptPath), "")]
        [DefaultValue(false)]
        public bool List
        {
            get; set;
        }

        [CommandProperty("entry")]
        [CommandPropertyTrigger(nameof(List), false)]
        [DefaultValue("")]
        public string ScriptEntry
        {
            get;
            set;
        }

        [CommandPropertyArray]
        public string[] Arguments
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

        protected override void OnExecute()
        {
            this.application.Culture = this.Culture;
            this.application.Verbose = LogVerbose.None;

            if (this.List == true)
            {
                Console.Write(this.ScriptContext.GenerateDeclaration(this.GetArgumentTypes()));
            }
            else
            {
                if (File.Exists(this.ScriptPath) == false)
                    throw new FileNotFoundException(this.ScriptPath);
                this.ScriptContext.RunFromFile(this.ScriptPath, this.ScriptEntry, this.GetProperties(), null);
            }
        }

        private static string ExecuteCmd(string cmd, string arguments)
        {
            var cmdPath = GetCmdPath(cmd);

            var startInfo = new ProcessStartInfo(cmdPath, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            var process = Process.Start(startInfo);
            var text = process.StandardOutput.ReadLine();
            process.WaitForExit();
            if (process.ExitCode == 2)
            {
                throw new Exception(text);
            }
            else if (process.ExitCode != 0)
            {
                throw new Exception(process.StandardError.ReadLine());
            }
            return text;
        }

        private static string GetCmdPath(string cmd)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var startInfo = new ProcessStartInfo("where", cmd + ".cmd")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                var process = Process.Start(startInfo);
                var text = process.StandardOutput.ReadLine();
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new Exception(process.StandardError.ReadLine());
                return text;
            }
            else
            {
                var startInfo = new ProcessStartInfo("which", cmd)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                var process = Process.Start(startInfo);
                var text = process.StandardOutput.ReadLine();
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new Exception(process.StandardError.ReadLine());
                return text;
            }
        }

        private IDictionary<string, object> GetProperties()
        {
            return CommandStringUtility.ArgumentsToDictionary(this.Arguments);
        }

        private Dictionary<string, Type> GetArgumentTypes()
        {
            var properties = new Dictionary<string, Type>(this.Arguments.Length);
            foreach (var item in this.Arguments)
            {
                if (CommandStringUtility.TryGetKeyValue(item, out var key, out var value) == true)
                {
                    var typeName = value;
                    if (CommandStringUtility.IsWrappedOfQuote(value))
                    {
                        value = CommandStringUtility.TrimQuot(value);
                    }

                    if (value == "number")
                    {
                        properties.Add(key, typeof(decimal));
                    }
                    else if (value == "boolean")
                    {
                        properties.Add(key, typeof(bool));
                    }
                    else if (value == "string")
                    {
                        properties.Add(key, typeof(string));
                    }
                    else
                    {
                        throw new ArgumentException(typeName);
                    }
                }
                else
                {
                    throw new ArgumentException(item);
                }
            }
            return properties;
        }

        public bool Parse(string key, string value)
        {
            //Regex.Match("--[a-zA-Z_$][a-zA-Z0-9_$]*[=])
            return false;
        }

        private ScriptContextBase ScriptContext => this.scriptContext.Value;
    }
}