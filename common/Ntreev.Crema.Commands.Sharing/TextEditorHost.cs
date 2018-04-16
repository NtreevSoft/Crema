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
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands
{
    static class TextEditorHost
    {
        private static string fileName;
        private static string arguments;

        static TextEditorHost()
        {
            UseShellExecute = false;
        }

        public static string FileName
        {
            get => fileName ?? string.Empty;
            set => fileName = value;
        }

        public static string Arguments
        {
            get => arguments ?? string.Empty;
            set => arguments = value;
        }

        public static bool UseShellExecute
        {
            get; set;
        }

        public static void Execute(string filename)
        {
            var startInfo = new ProcessStartInfo();
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                if (FileName != string.Empty)
                {
                    startInfo.UseShellExecute = UseShellExecute;
                    startInfo.FileName = FileName;
                    startInfo.Arguments = $"{filename} {Arguments}";
                }
                else
                {
                    startInfo.FileName = @"vi";
                    startInfo.Arguments = $"{filename}";
                }
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (FileName != string.Empty)
                {
                    startInfo.UseShellExecute = UseShellExecute;
                    startInfo.FileName = FileName;
                    startInfo.Arguments = $"{filename} {Arguments}";
                }
                else
                {
                    startInfo.UseShellExecute = UseShellExecute;
                    startInfo.FileName = "notepad";
                    startInfo.Arguments = $"{filename}";
                }
            }

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }

        public static Task ExecuteAsync(string filename)
        {
            return Task.Run(() => Execute(filename));
        }

        [Export(typeof(IConfigurationPropertyProvider))]
        class PropertiesProvider : IConfigurationPropertyProvider
        {
            public string Name => "textEditor";

            [ConfigurationProperty]
            [DefaultValue("")]
            public string FileName
            {
                get => TextEditorHost.FileName;
                set => TextEditorHost.FileName = value;
            }

            [ConfigurationProperty]
            [DefaultValue("")]
            public string Arguments
            {
                get => TextEditorHost.Arguments;
                set => TextEditorHost.Arguments = value;
            }
        }
    }
}
