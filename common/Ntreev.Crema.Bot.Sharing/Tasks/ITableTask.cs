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
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(ITableTask))]
    [TaskClass]
    public class ITableTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var table = context.Target as ITable;
            if (context.IsCompleted(table) == true)
            {
                context.Pop(table);
            }
            else if (RandomUtility.Within(75) == true)
            {
                var content = table.Dispatcher.Invoke(() => table.Content);
                context.Push(content, RandomUtility.Next(100));
            }
            else if (RandomUtility.Within(75) == true)
            {
                var template = table.Dispatcher.Invoke(() => table.Template);
                context.Push(template);
            }
        }

        public Type TargetType
        {
            get { return typeof(ITable); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod]
        public void GetAccessType(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                table.GetAccessType(context.Authentication);
            });
        }

        //[TaskMethod]
        //public void VerifyRead(ITable table, TaskContext context)
        //{
        //    table.Dispatcher.Invoke(() =>
        //    {
        //        table.VerifyRead(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyOwner(ITable table, TaskContext context)
        //{
        //    table.Dispatcher.Invoke(() =>
        //    {
        //        table.VerifyOwner(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyMember(ITable table, TaskContext context)
        //{
        //    table.Dispatcher.Invoke(() =>
        //    {
        //        table.VerifyMember(context.Authentication);
        //    });
        //}

        [TaskMethod(Weight = 10)]
        public void Lock(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var comment = RandomUtility.NextString();
                if (Verify(comment) == true)
                    return;
                table.Lock(context.Authentication, comment);
            });

            bool Verify(string comment)
            {
                if (context.AllowException == true)
                    return true;
                if (comment == string.Empty)
                    return false;
                if (table.IsLocked == true)
                    return false;
                return false;
            }
        }

        [TaskMethod]
        public void Unlock(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                if (Verify() == false)
                    return;
                table.Unlock(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (table.IsLocked == false)
                    return false;
                return false;
            }
        }

        [TaskMethod]
        public void SetPublic(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                if (Verify() == false)
                    return;
                table.SetPublic(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (table.IsPrivate == false)
                    return false;
                return false;
            }
        }

        [TaskMethod(Weight = 10)]
        public void SetPrivate(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                if (Verify() == false)
                    return;
                table.SetPrivate(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (table.IsPrivate == true)
                    return false;
                return false;
            }
        }

        [TaskMethod(Weight = 10)]
        public void AddAccessMember(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var userContext = table.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                var accessType = RandomUtility.NextEnum<AccessType>();
                table.AddAccessMember(context.Authentication, memberID, accessType);
            });
        }

        [TaskMethod]
        public void RemoveAccessMember(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var userContext = table.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                table.RemoveAccessMember(context.Authentication, memberID);
            });
        }

        [TaskMethod]
        public void Rename(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var tableName = RandomUtility.NextIdentifier();
                table.Rename(context.Authentication, tableName);
            });
        }

        [TaskMethod]
        public void Move(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var categories = table.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                var categoryPath = categories.Random().Path;
                table.Move(context.Authentication, categoryPath);
            });
        }

        [TaskMethod(Weight = 1)]
        public void Delete(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
            });
        }

        [TaskMethod]
        public void SetTags(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var tags = (TagInfo)TagInfoUtility.Names.Random();
                table.SetTags(context.Authentication, tags);
            });
        }

        [TaskMethod]
        public void Copy(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var categories = table.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                var categoryPath = categories.Random().Path;
                var tableName = RandomUtility.NextIdentifier();
                var copyData = RandomUtility.NextBoolean();
                table.Copy(context.Authentication, tableName, categoryPath, copyData);
            });
        }

        [TaskMethod]
        public void Inherit(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var categories = table.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                var categoryPath = categories.Random().Path;
                var tableName = RandomUtility.NextIdentifier();
                var copyData = RandomUtility.NextBoolean();
                table.Inherit(context.Authentication, tableName, categoryPath, copyData);
            });
        }

        [TaskMethod]
        public void NewTable(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var template = table.NewTable(context.Authentication);
                context.Push(template);
            });
        }

        [TaskMethod]
        public void Preview(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                table.GetDataSet(context.Authentication, null);
            });
        }

        [TaskMethod]
        public void GetLog(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                table.GetLog(context.Authentication);
            });
        }

        [TaskMethod]
        public void Find(ITable table, TaskContext context)
        {
            table.Dispatcher.Invoke(() =>
            {
                var text = RandomUtility.NextWord();
                var option = RandomUtility.NextEnum<FindOptions>();
                table.Find(context.Authentication, text, option);
            });
        }
    }
}
