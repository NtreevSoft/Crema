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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Test
{
    class CremaServeHost
    {
        private const string filename = @"..\..\..\..\bin\Debug\cremaserve\cremaserve.exe";
        //private const string filename = @"D:\Projects\Git\Crema\bin\Debug\cremaserve\cremaserve.exe";
        private static readonly StringBuilder sb = new StringBuilder();

        public static int ExitCode
        {
            get; private set;
        }

        public static string OutputString
        {
            get { return sb.ToString(); }
        }

        public static void Run(params object[] args)
        {
            sb.Clear();
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = filename;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.Arguments = string.Join(" ", args);
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            //var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            ExitCode = process.ExitCode;
            //OutputString = output;
            //return output;
        }

        public static Process RunAsync(params object[] args)
        {
            sb.Clear();
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = filename;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.Arguments = string.Join(" ", args);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            //var output = process.StandardOutput.ReadToEnd();
            //process.WaitForExit();
            //ExitCode = process.ExitCode;
            //OutputString = output;
            //return output;

            return process;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            sb.AppendLine(e.Data);
            Console.WriteLine(e.Data);
        }
    }
}
