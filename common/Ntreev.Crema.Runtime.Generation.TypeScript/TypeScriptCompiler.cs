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

using Microsoft.CSharp;
using Ntreev.Crema.Runtime.Generation.TypeScript.TypeScript;
using Ntreev.Crema.Runtime.Generation.TypeScript.Properties;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.Diagnostics;
using Ntreev.Library.IO;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Runtime.Generation.TypeScript
{
    static class TypeScriptCompiler
    {
        public static IDictionary<string, string> Compile(IDictionary<string, string> codes, string target)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName().Replace(".", "_"));
            var nodePath = Path.Combine(tempPath, "typings", "node", "node.d.ts");

            try
            {
                DirectoryUtility.PrepareFile(tempPath + "/");
                DirectoryUtility.PrepareFile(nodePath);
                //File.WriteAllText(nodePath, Resources.NodeDeclaration);

                var query = from item in codes
                            where item.Key.EndsWith(".d.ts") == false
                            select item;

                WriteFiles(tempPath, query);
                CompileFiles(tempPath, query, target);
                ReplaceCodes(tempPath, codes, query);

                return codes;
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        private static void ReplaceCodes(string tempPath, IDictionary<string, string> codes, IEnumerable<KeyValuePair<string, string>> query)
        {
            foreach (var item in query.ToArray())
            {
                codes.Remove(item.Key);
                var path = Path.Combine(tempPath, item.Key);
                path = Path.ChangeExtension(path, ".js");
                var code = File.ReadAllText(path);
                codes.Add(Path.GetFileName(path), code);
            }
        }

        private static void CompileFiles(string tempPath, IEnumerable<KeyValuePair<string, string>> query, string target)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempPath);
                var fileArgs = string.Join(" ", query.Select(item => $"\"{Path.Combine(tempPath, item.Key)}\""));
                var arguments = new Dictionary<string, string>
                {
                    { "sourcemap", string.Empty },
                    { "module", "commonjs" },
                    { "target", target },
                    { "outDir", "\"" + tempPath + "\"" }
                };
                var args = string.Join(" ", arguments.Select(item => string.Format("--{0} {1}", item.Key, item.Value)));

                ExecuteCmd("npm", "install --save-dev @types/node");
                ExecuteCmd("tsc", args + " " + fileArgs);
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDirectory);
            }
        }

        private static void WriteFiles(string tempPath, IEnumerable<KeyValuePair<string, string>> query)
        {
            foreach (var item in query)
            {
                var path = Path.Combine(tempPath, item.Key);
                using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.WriteLine(item.Value);
                }
            }
        }

        private static void ExecuteCmd(string cmd, string arguments)
        {
            var cmdPath = GetCmdPath(cmd);

            var startInfo = new ProcessStartInfo(cmdPath, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var process = Process.Start(startInfo);
            Console.WriteLine(process.StandardOutput.ReadLine());
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception(process.StandardError.ReadLine());
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
    }
}