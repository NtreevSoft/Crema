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

using Ntreev.Library.Commands;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Console
{
    class ConsoleCommandControl : TerminalControl
    {
        private readonly static string[] emptyStrings = new string[] { };
        private ConsoleCommandContext commandContext;

        public ConsoleCommandControl()
        {
            this.SetResourceReference(StyleProperty, typeof(TerminalControl));
        }

        public ConsoleCommandContext CommandContext
        {
            get => this.commandContext;
            set => this.commandContext = value;
        }

        protected override string[] GetCompletion(string[] items, string find)
        {
            if (items.Length == 0)
            {
                var query = from item in this.commandContext.Commands
                            let name = item.Name
                            where this.commandContext.IsCommandEnabled(item)
                            where name.StartsWith(find)
                            select name;
                return query.ToArray();
            }
            else if (items.Length == 1)
            {
                var commandName = items[0];
                var command = this.GetCommand(commandName);
                if (command == null)
                    return null;
                if (command is IExecutable == false)
                {
                    var query = from item in CommandDescriptor.GetMethodDescriptors(command)
                                let name = item.Name
                                where this.commandContext.IsMethodEnabled(command, item)
                                where name.StartsWith(find)
                                select name;
                    return query.ToArray();
                }
                else
                {
                    var memberList = new List<CommandMemberDescriptor>(CommandDescriptor.GetMemberDescriptors(command));
                    var argList = new List<string>(items.Skip(1));
                    var completionContext = new CommandCompletionContext(command, memberList, argList, find);
                    return this.GetCompletions(completionContext);
                }
            }
            else if (items.Length >= 2)
            {
                var commandName = items[0];
                var command = this.GetCommand(commandName);
                if (command == null)
                    return null;

                if (command is IExecutable == true)
                {
                    var memberList = new List<CommandMemberDescriptor>(CommandDescriptor.GetMemberDescriptors(command));
                    var argList = new List<string>(items.Skip(1));
                    var completionContext = new CommandCompletionContext(command, memberList, argList, find);
                    return this.GetCompletions(completionContext);
                }
                else
                {
                    var methodName = items[1];
                    var methodDescriptor = this.GetMethodDescriptor(command, methodName);
                    if (methodDescriptor == null)
                        return null;

                    if (methodDescriptor.Members.Length == 0)
                        return null;

                    var commandTarget = this.GetCommandTarget(command, methodDescriptor);
                    var memberList = new List<CommandMemberDescriptor>(methodDescriptor.Members);
                    var argList = new List<string>(items.Skip(2));
                    var completionContext = new CommandCompletionContext(command, methodDescriptor, memberList, argList, find);
                    if (completionContext.MemberDescriptor == null && find != string.Empty)
                        return this.GetCompletions(memberList, find);

                    return this.GetCompletions(completionContext);
                }
            }

            return null;
        }

        protected virtual string[] GetCompletions(CommandCompletionContext completionContext)
        {
            var query = from item in GetCompletionsCore() ?? emptyStrings
                        where item.StartsWith(completionContext.Find)
                        select item;
            return query.ToArray();

            string[] GetCompletionsCore()
            {
                if (completionContext.Command is CommandBase commandBase)
                {
                    return commandBase.GetCompletions(completionContext);
                }
                else if (completionContext.Command is CommandMethodBase commandMethodBase)
                {
                    return commandMethodBase.GetCompletions(completionContext.MethodDescriptor, completionContext.MemberDescriptor, completionContext.Find);
                }
                else if (completionContext.Command is CommandProviderBase consoleCommandProvider)
                {
                    return consoleCommandProvider.GetCompletions(completionContext.MethodDescriptor, completionContext.MemberDescriptor);
                }
                return null;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Ntreev.Library.Commands.Terminal.BufferWidth = (int)sizeInfo.NewSize.Width / 6;
        }

        private CommandMemberDescriptor FindMemberDescriptor(List<string> argList, List<CommandMemberDescriptor> memberList)
        {
            if (argList.Any())
            {
                var arg = argList.Last();
                var descriptor = this.FindMemberDescriptor(memberList, arg);
                if (descriptor != null)
                {
                    return descriptor;
                }
            }

            for (var i = 0; i < argList.Count; i++)
            {
                if (memberList.Any() == false)
                    break;
                var arg = argList[i];
                var member = memberList.First();
                if (member.IsRequired == true)
                    memberList.RemoveAt(0);
            }

            if (memberList.Any() == true && memberList.First().IsRequired == true)
            {
                return memberList.First();
            }
            return null;
        }

        private ICommand GetCommand(string commandName)
        {
            var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();
            if (Enumerable.Contains(commandNames, commandName) == true)
            {
                var command = this.commandContext.Commands[commandName];
                if (this.commandContext.IsCommandEnabled(command) == true)
                    return command;
            }
            if (commandName == this.commandContext.HelpCommand.Name)
                return this.commandContext.HelpCommand;
            return null;
        }

        private CommandMethodDescriptor GetMethodDescriptor(ICommand command, string methodName)
        {
            if (command is IExecutable == true)
                return null;
            var descriptors = CommandDescriptor.GetMethodDescriptors(command);
            if (descriptors.Contains(methodName) == false)
                return null;
            var descriptor = descriptors[methodName];
            if (this.commandContext.IsMethodEnabled(command, descriptor) == false)
                return null;
            return descriptor;
        }

        private CommandMemberDescriptor FindMemberDescriptor(IEnumerable<CommandMemberDescriptor> descriptors, string argument)
        {
            foreach (var item in descriptors)
            {
                if (item.NamePattern == argument || item.ShortNamePattern == argument)
                {
                    return item;
                }
            }
            return null;
        }

        private object GetCommandTarget(ICommand command, CommandMethodDescriptor methodDescriptor)
        {
            var methodInfo = methodDescriptor.MethodInfo;
            if (methodInfo.DeclaringType == command.GetType())
                return command;
            var query = from item in this.commandContext.CommandProviders
                        where item.CommandName == command.Name
                        where item.GetType() == methodInfo.DeclaringType
                        select item;

            return query.First();
        }

        private string[] GetCompletions(IEnumerable<CommandMemberDescriptor> descriptors, string find)
        {
            var patternList = new List<string>();
            foreach (var item in descriptors)
            {
                if (item.IsRequired == false)
                {
                    if (item.NamePattern != string.Empty)
                        patternList.Add(item.NamePattern);
                    if (item.ShortNamePattern != string.Empty)
                        patternList.Add(item.ShortNamePattern);
                }
            }
            return patternList.Where(item => item.StartsWith(find)).ToArray();
        }
    }
}
