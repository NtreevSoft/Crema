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

using Ntreev.Crema.Commands.Consoles.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class BanCommand : UserCommandBase, IConsoleCommand
    {
        public BanCommand()
            : base("ban")
        {

        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor != null && completionContext.MemberDescriptor.DescriptorName == nameof(this.UserID))
            {
                return this.GetUserList();
            }
            return base.GetCompletions(completionContext);
        }

        [CommandProperty(IsRequired = true)]
        public string UserID
        {
            get; set;
        }

        [CommandProperty('m')]
        [CommandPropertyTrigger(nameof(Information), false)]
        [DefaultValue("")]
        public string Comment
        {
            get; set;
        }

        [CommandProperty('i')]
        [CommandPropertyTrigger(nameof(Comment), "")]
        public bool Information
        {
            get; set;
        }

        [CommandProperty("format")]
        [CommandPropertyTrigger(nameof(Information), true)]
        [DefaultValue(TextSerializerType.Yaml)]
        public TextSerializerType FormatType
        {
            get; set;
        }

        protected override void OnExecute()
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var user = this.GetUser(authentication, this.UserID);
            if (this.Information == true)
            {
                var banInfo = user.Dispatcher.Invoke(() => user.BanInfo);
                var prop = banInfo.ToDictionary();
                var text = TextSerializer.Serialize(prop, this.FormatType);
                this.Out.WriteLine(text);
            }
            else
            {
                if (this.Comment == string.Empty)
                {
                    throw new ArgumentException($"'{this.GetDescriptor(nameof(this.Comment)).DisplayPattern}' 가 필요합니다.");
                }
                user.Dispatcher.Invoke(() => user.Ban(authentication, this.Comment));
            }
        }
    }
}
