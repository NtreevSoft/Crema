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
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands.Consoles
{
    public abstract class ConsoleTerminalBase : CommandContextTerminal
    {
        private readonly ConsoleCommandContextBase commandContext;

        protected ConsoleTerminalBase(ConsoleCommandContextBase commandContext)
            : base(commandContext)
        {
            this.commandContext = commandContext;
            this.commandContext.PathChanged += CommandContext_PathChanged;
            this.commandContext.Executed += CommandContext_Executed;
            this.commandContext.Terminal = this;
            this.Postfix = this.PostfixInternal;
            this.IsCommandMode = true;
        }

        public bool IsCommandMode
        {
            get; set;
        }

        protected override void OnDrawPrompt(TextWriter writer, string prompt)
        {
            if (this.IsCommandMode == false)
            {
                base.OnDrawPrompt(writer, prompt);
            }
            else
            {
                if (prompt == string.Empty)
                    return;
                var postfixPattern = string.Join(string.Empty, this.Postfix.Select(item => $"[{item}]"));
                if (this.commandContext.IsOnline == false)
                {
                    var match = Regex.Match(prompt, $"(.+)(?<postfix>{postfixPattern})$");
                    using (TerminalColor.SetForeground(ConsoleColor.Green))
                    {
                        writer.Write(match.Groups[1].Value);
                    }
                    Console.ResetColor();
                    writer.Write(match.Groups[2].Value);
                }
                else
                {
                    var uri = new Uri(prompt);
                    //var match = Regex.Match(prompt, $"(.+)([:])({PathUtility.Separator}.*)({postfixPattern})$");
                    using (TerminalColor.SetForeground(ConsoleColor.Green))
                    {
                        writer.Write(uri.Scheme);
                    }
                    writer.Write(Uri.SchemeDelimiter);
                    using (TerminalColor.SetForeground(ConsoleColor.Cyan))
                    {
                        writer.Write(uri.UserInfo);
                    }
                    writer.Write("@");
                    using (TerminalColor.SetForeground(ConsoleColor.Cyan))
                    {
                        writer.Write(uri.Host);
                    }
                    Console.ResetColor();
                    writer.Write(uri.LocalPath);
                }
            }
        }

        private void CommandContext_Executed(object sender, EventArgs e)
        {
            this.SetPrompt();
        }

        private void CommandContext_PathChanged(object sender, EventArgs e)
        {
            this.SetPrompt();
        }

        protected void SetPrompt()
        {
            base.Prompt = this.commandContext.Prompt;
        }

        private string PostfixInternal
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    return "$ ";
                }
                else
                {
                    return ">";
                }
            }
        }
    }
}
