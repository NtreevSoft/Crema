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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Crema.Services.Random;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(ITypeMemberTask))]
    [TaskClass]
    class ITypeMemberTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var member = context.Target as ITypeMember;
            member.Dispatcher.Invoke(() =>
            {
                var template = member.Template;
                if (context.IsCompleted(member) == true)
                {
                    if (object.Equals(context.State, System.Data.DataRowState.Detached) == true)
                    {
                        try
                        {
                            template.EndNew(context.Authentication, member);
                        }
                        catch
                        {
                            
                        }
                    }

                    context.State = null;
                    context.Pop(member);
                }
            });
        }

        public Type TargetType
        {
            get { return typeof(ITypeMember); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod(Weight = 1)]
        public void Delete(ITypeMember member, TaskContext context)
        {
            member.Dispatcher.Invoke(() =>
            {
                if (object.Equals(context.State, System.Data.DataRowState.Detached) == false)
                {
                    member.Delete(context.Authentication);
                    context.State = System.Data.DataRowState.Deleted;
                    context.Complete(member);
                }
            });
        }

        [TaskMethod]
        public void SetIndex(ITypeMember member, TaskContext context)
        {
            member.Dispatcher.Invoke(() =>
            {
                var index = RandomUtility.Next(member.Template.Count);
                member.SetIndex(context.Authentication, index);
            });
        }

        [TaskMethod(Weight = 20)]
        public void SetName(ITypeMember member, TaskContext context)
        {
            member.Dispatcher.Invoke(() =>
            {
                var memberName = RandomUtility.NextIdentifier();
                member.SetName(context.Authentication, memberName);
            });
        }

        [TaskMethod]
        public void SetValue(ITypeMember member, TaskContext context)
        {
            member.Dispatcher.Invoke(() =>
            {
                var template = member.Template;
                if (template.IsFlag == true)
                {
                    var value = RandomUtility.NextBit();
                    member.SetValue(context.Authentication, value);
                }
                else
                {
                    var value = RandomUtility.NextLong(long.MaxValue);
                    member.SetValue(context.Authentication, value);
                }
            });
        }

        [TaskMethod]
        public void SetComment(ITypeMember member, TaskContext context)
        {
            member.Dispatcher.Invoke(() =>
            {
                var comment = RandomUtility.NextString();
                member.SetComment(context.Authentication, comment);
            });
        }
    }
}
