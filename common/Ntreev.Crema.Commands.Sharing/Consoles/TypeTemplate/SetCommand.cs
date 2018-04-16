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
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using Ntreev.Crema.Commands.Consoles.Serializations;
using Ntreev.Crema.Commands.Consoles.Properties;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Ntreev.Crema.Commands.Consoles.TypeTemplate
{
    [Export(typeof(ITemplateCommand))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [CommandStaticProperty(typeof(TextEditorHost))]
    class SetCommand : TemplateCommandBase, ITemplateCommand
    {
        public SetCommand()
            : base("set")
        {

        }

        public ITypeTemplate Template { get { return this.CommandContext.Template; } }

        public Authentication Authentication { get { return this.CommandContext.Authentication; } }

        [CommandProperty]
        [DefaultValue("")]
        public string MemberName
        {
            get; set;
        }

        protected override void OnExecute()
        {
            if (this.MemberName == string.Empty)
            {
                this.EditMembers();
            }
            else
            {
                this.EditMember();
            }
        }

        public override string[] GetCompletions(string find)
        {
            return base.GetCompletions(find);
        }

        private void EditMembers()
        {
            var memberCount = this.Template.Dispatcher.Invoke(() => this.Template.Count);
            var memberList = new List<JsonTypeMemberInfos.ItemInfo>(memberCount);
            var idToMember = new Dictionary<Guid, ITypeMember>(memberCount);

            this.Template.Dispatcher.Invoke(() =>
            {
                foreach (var item in this.Template)
                {
                    var member = new JsonTypeMemberInfos.ItemInfo()
                    {
                        ID = Guid.NewGuid(),
                        Name = item.Name,
                        Value = item.Value,
                        Comment = item.Comment,
                    };
                    idToMember.Add(member.ID, item);
                    memberList.Add(member);
                }
            });

            var members = new JsonTypeMemberInfos() { Items = memberList.ToArray() };

            using (var editor = new JsonEditorHost(members))
            {
                if (editor.Execute() == false)
                    return;

                members = editor.Read<JsonTypeMemberInfos>();
            }

            this.Template.Dispatcher.Invoke(() =>
            {
                foreach (var item in idToMember.Keys.ToArray())
                {
                    if (members.Items.Any(i => i.ID == item) == false)
                    {
                        var member = idToMember[item];
                        member.Delete(this.Authentication);
                        idToMember.Remove(item);
                    }
                }

                for (var i = 0; i < members.Items.Length; i++)
                {
                    var item = members.Items[i];
                    if (item.ID == Guid.Empty)
                    {
                        var member = this.Template.AddNew(this.Authentication);
                        member.SetName(this.Authentication, item.Name);
                        member.SetValue(this.Authentication, item.Value);
                        member.SetComment(this.Authentication, item.Comment);
                        this.Template.EndNew(this.Authentication, member);
                        item.ID = Guid.NewGuid();
                        idToMember.Add(item.ID, member);
                        members.Items[i] = item;
                    }
                    else if (idToMember.ContainsKey(item.ID) == true)
                    {
                        var member = idToMember[item.ID];
                        if (member.Name != item.Name)
                            member.SetName(this.Authentication, item.Name);
                        if (member.Value != item.Value)
                            member.SetValue(this.Authentication, item.Value);
                        if (member.Comment != item.Comment)
                            member.SetComment(this.Authentication, item.Comment);
                    }
                    else
                    {
                        throw new InvalidOperationException($"{item.ID} is not existed member.");
                    }
                }

                for (var i = 0; i < members.Items.Length; i++)
                {
                    var item = members.Items[i];
                    var member = idToMember[item.ID];
                    member.SetIndex(this.Authentication, i);
                }
            });

            //member.Dispatcher.Invoke(() =>
            //{
            //    if (member.Name != value.Name)
            //        member.SetName(this.Authentication, value.Name);
            //    if (member.Value != value.Value)
            //        member.SetValue(this.Authentication, value.Value);
            //    if (member.Comment != value.Comment)
            //        member.SetComment(this.Authentication, value.Comment);
            //    if (member.Index != value.Index)
            //        member.SetIndex(this.Authentication, value.Index);
            //});
        }

        private void EditMember()
        {
            var value = new JsonTypeMemberInfo();
            var member = this.Template.Dispatcher.Invoke(() => this.Template[this.MemberName]);
            var memberCount = this.Template.Dispatcher.Invoke(() => this.Template.Count);

            this.Template.Dispatcher.Invoke(() =>
            {
                value.Name = member.Name;
                value.Index = member.Index;
                value.Value = member.Value;
                value.Comment = member.Comment;
            });

            var schema = JsonSchemaUtility.CreateSchema(typeof(JsonTypeMemberInfo));
            var indexSchema = schema.Properties[nameof(JsonTypeMemberInfo.Index)];
            indexSchema.Minimum = 0;
            indexSchema.Maximum = memberCount - 1;

            using (var editor = new JsonEditorHost(value, schema))
            {
                if (editor.Execute() == true)
                {
                    value = editor.Read<JsonTypeMemberInfo>();
                    member.Dispatcher.Invoke(() =>
                    {
                        if (member.Name != value.Name)
                            member.SetName(this.Authentication, value.Name);
                        if (member.Value != value.Value)
                            member.SetValue(this.Authentication, value.Value);
                        if (member.Comment != value.Comment)
                            member.SetComment(this.Authentication, value.Comment);
                        if (member.Index != value.Index)
                            member.SetIndex(this.Authentication, value.Index);
                    });
                }
            }
        }
    }
}
