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

using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class ListCommand : ConsoleCommandBase
    {
        public ListCommand()
            : base("ls")
        {

        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor == null || completionContext.MemberDescriptor.DescriptorName == nameof(PathName) == true)
                return this.CommandContext.GetCompletion(completionContext.Find);
            return base.GetCompletions(completionContext);
        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public string PathName
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.IsOnline && this.CommandContext.Drive != null;

        protected override void OnExecute()
        {
            var allPath = this.CommandContext.Drive.GetPaths();
            var path = this.PathName == string.Empty ? this.CommandContext.Path : this.CommandContext.GetAbsolutePath(this.PathName);

            if (path.EndsWith(PathUtility.Separator) == false)
                path += PathUtility.Separator;

            if (allPath.Contains(path) == false)
                throw new ArgumentException($"No such directory : {path}");

            var query = from item in allPath
                        where item.StartsWith(path)
                        let text = item.Substring(path.Length)
                        where StringUtility.Split(text, PathUtility.SeparatorChar).Length == 1
                        let name = text.Trim(PathUtility.SeparatorChar)
                        orderby name
                        select new ItemObject(text);

            this.CommandContext.WriteList(query.ToArray());
        }

        #region classes

        class ItemObject : TerminalTextItem
        {
            private bool isCategory;
            private string name;
            public ItemObject(string path)
                : base(path)
            {
                if (path.EndsWith(PathUtility.Separator) == true)
                {
                    this.name = path.Remove(path.Length - 1);
                    this.isCategory = true;
                }
                else
                {
                    this.name = path;
                    this.isCategory = false;
                }
            }

            public bool IsCategory { get { return this.isCategory; } }

            protected override void OnDraw(TextWriter writer, string text)
            {
                if (this.isCategory == true)
                {
                    using (TerminalColor.SetForeground(ConsoleColor.Cyan))
                    {
                        base.OnDraw(writer, text);
                    }
                }
                else
                {
                    base.OnDraw(writer, text);
                }
            }

            public override string ToString()
            {
                return this.name;
            }
        }

        #endregion
    }
}
