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
    class SvnAdminCommand : IEnumerable<object>
    {
        private const string svnadmin = "svnadmin";
        private readonly string commandName;
        private readonly List<object> items = new List<object>();

        public SvnAdminCommand(string commandName)
        {
            this.commandName = commandName;
        }

        public override string ToString()
        {
            return $"{svnadmin} {this.commandName} {string.Join(" ", this.items)}";
        }

        public void Add(object item)
        {
            this.items.Add(item);
        }

        public string Run()
        {
            var sb = new StringBuilder();
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = svnadmin;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = $"{this.commandName} {string.Join(" ", this.items)}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.OutputDataReceived += (s, e) =>
            {
                sb.AppendLine(e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception(sb.ToString());

            return sb.ToString();
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