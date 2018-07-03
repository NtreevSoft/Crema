using Ntreev.Crema.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    class SvnCommand : IEnumerable<object>
    {
        private const string svn = "svn";
        private readonly string commandName;
        private readonly List<object> items = new List<object>();

        public SvnCommand(string commandName)
        {
            this.commandName = commandName;
        }

        public override string ToString()
        {
            return $"{svn} {this.commandName} {string.Join(" ", this.items)}";
        }

        public void Add(object item)
        {
            this.items.Add(item);
        }

        public string Run()
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = svn;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = $"{this.commandName} {string.Join(" ", this.items)}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.OutputDataReceived += (s, e) =>
            {
                outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                errorBuilder.AppendLine(e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception(errorBuilder.ToString());

            return outputBuilder.ToString();
        }

        public string Run(ILogService logService)
        {
            try
            {
                return this.Run();
            }
            catch (Exception e)
            {
                logService.Error(e);
                throw;
            }
        }

        public string[] ReadLines()
        {
            return this.ReadLines(false);
        }

        public string[] ReadLines(bool removeEmptyLine)
        {
            var lines = this.Run();
            return this.GetLines(lines, removeEmptyLine);
        }

        private string[] GetLines(string text, bool removeEmptyLine)
        {
            using (var sr = new StringReader(text))
            {
                var line = null as string;
                var lineList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() != string.Empty || removeEmptyLine == false)
                    {
                        lineList.Add(line);
                    }
                }
                return lineList.ToArray();
            }
        }

        #region IEnumerable

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach (var item in this.items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.items)
            {
                yield return item;
            }
        }

        #endregion
    }
}