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

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(ITableContentTask))]
    [TaskClass]
    class ITableContentTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var content = context.Target as ITableContent;
            content.Dispatcher.Invoke(() =>
            {
                if (context.IsCompleted(content) == true)
                {
                    var domain = content.Domain;
                    if (domain != null && RandomUtility.Within(50) == true)
                    {
                        var isMember = domain.Dispatcher.Invoke(() =>
                        {
                            if (domain.Users.Contains(context.Authentication.ID) == false)
                                return false;
                            var user = domain.Users[context.Authentication.ID];
                            return user.DomainUserState.HasFlag(DomainUserState.Online);
                        });
                        if (isMember == true)
                        {
                            content.LeaveEdit(context.Authentication);
                        }
                        var isEmpty = domain.Dispatcher.Invoke(() => domain.Users.Any() == false);
                        if (isEmpty == true)
                        {
                            content.EndEdit(context.Authentication);
                        }
                    }
                    context.Pop(content);
                    context.Complete(context.Target);
                }
                else
                {
                    if (content.Domain == null)
                        content.BeginEdit(context.Authentication);

                    var domain = content.Domain;
                    var isMember = domain.Dispatcher.Invoke(() =>
                    {
                        if (domain.Users.Contains(context.Authentication.ID) == false)
                            return false;
                        var user = domain.Users[context.Authentication.ID];
                        return user.DomainUserState.HasFlag(DomainUserState.Online);
                    });
                    if (isMember == false)
                    {
                        content.EnterEdit(context.Authentication);
                    }

                    if (content.Any() == false || RandomUtility.Within(25) == true)
                    {
                        var row = this.AddNewRow(context.Authentication, content);
                        if (row != null)
                        {
                            row.InitializeRandom(context.Authentication);
                            context.Push(row);
                            context.State = System.Data.DataRowState.Detached;
                        }
                    }
                    else
                    {
                        var member = content.Random();
                        context.Push(member);
                    }
                }
            });
        }

        private ITableRow AddNewRow(Authentication authentication, ITableContent content)
        {
            var table = content.Table;
            var parentContent = table.Parent.Content;
            if (parentContent != null)
            {
                if (parentContent.Any() == false)
                    return null;
                var relationID = parentContent.Random().RelationID;
                return content.AddNew(authentication, relationID);
            }
            return content.AddNew(authentication, null);
        }

        public Type TargetType
        {
            get { return typeof(ITableContent); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }
    }
}
