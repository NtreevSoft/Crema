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
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(ITableCategoryTask))]
    [TaskClass]
    public class ITableCategoryTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var category = context.Target as ITableCategory;
            category.Dispatcher.Invoke(() =>
            {
                var tables = category.GetService(typeof(ITableCollection)) as ITableCollection;
                var categories = category.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                if (context.IsCompleted(category) == true)
                {
                    context.Pop(category);
                }
                else if (categories.Count < RandomUtility.Next(Math.Max(10, categories.Count + 1)))
                {
                    this.AddNewCategory(category, context);
                    context.Complete(category);
                }
                else if (tables.Count < RandomUtility.Next(Math.Max(10, tables.Count + 1)))
                {
                    var template = category.NewTable(context.Authentication);
                    context.Push(template);
                }
                else
                {
                    context.Complete(category);
                }
            });
        }

        public Type TargetType
        {
            get { return typeof(ITableCategory); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod]
        public void GetAccessType(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                category.GetAccessType(context.Authentication);
            });
        }

        //[TaskMethod]
        //public void VerifyRead(ITableCategory category, TaskContext context)
        //{
        //    category.Dispatcher.Invoke(() =>
        //    {
        //        category.VerifyRead(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyOwner(ITableCategory category, TaskContext context)
        //{
        //    category.Dispatcher.Invoke(() =>
        //    {
        //        category.VerifyOwner(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyMember(ITableCategory category, TaskContext context)
        //{
        //    category.Dispatcher.Invoke(() =>
        //    {
        //        category.VerifyMember(context.Authentication);
        //    });
        //}

        [TaskMethod(Weight = 10)]
        public void Lock(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var comment = RandomUtility.NextString();
                if (Verify(comment) == false)
                    return;
                category.Lock(context.Authentication, comment);
            });

            bool Verify(string comment)
            {
                if (context.AllowException == true)
                    return true;
                if (comment == string.Empty)
                    return false;
                if (category.IsLocked == true)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void Unlock(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                if (Verify() == false)
                    return;
                category.Unlock(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (category.IsLocked == false)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void SetPublic(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                if (Verify() == false)
                    return;
                category.SetPublic(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (category.IsPrivate == false)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 10)]
        public void SetPrivate(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                category.SetPrivate(context.Authentication);
            });
        }

        [TaskMethod(Weight = 10)]
        public void AddAccessMember(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var userContext = category.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                var accessType = RandomUtility.NextEnum<AccessType>();
                if (NameValidator.VerifyItemPath(memberID) == true)
                {
                    category.AddAccessMember(context.Authentication, new ItemName(memberID).Name, accessType);
                }
                else
                {
                    category.AddAccessMember(context.Authentication, memberID, accessType);
                }
            });
        }

        [TaskMethod(Weight = 10)]
        public void RemoveAccessMember(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var userContext = category.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                if (NameValidator.VerifyItemPath(memberID) == true)
                {
                    category.RemoveAccessMember(context.Authentication, new ItemName(memberID).Name);
                }
                else
                {
                    category.RemoveAccessMember(context.Authentication, memberID);
                }
            });
        }

        [TaskMethod]
        public void Rename(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var categoryName = RandomUtility.NextIdentifier();
                if (Verify() == false)
                    return;
                category.Rename(context.Authentication, categoryName);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (category.Parent == null)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void Move(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var categories = category.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                var categoryPath = categories.Random().Path;
                if (Verify(categoryPath) == false)
                    return;
                category.Move(context.Authentication, categoryPath);
            });

            bool Verify(string categoryPath)
            {
                if (context.AllowException == true)
                    return true;
                if (categoryPath.StartsWith(category.Path) == true)
                    return false;
                if (category.Parent.Path == categoryPath)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 1)]
        public void Delete(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
            });
        }

        [TaskMethod(Weight = 10)]
        public void AddNewCategory(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var categoryNanme = RandomUtility.NextIdentifier();
                category.AddNewCategory(context.Authentication, categoryNanme);
            });
        }

        [TaskMethod(Weight = 10)]
        public void NewTable(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var template = category.NewTable(context.Authentication);
                context.Push(template);
            });
        }

        [TaskMethod]
        public void Preview(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                category.GetDataSet(context.Authentication, null);
            });
        }

        [TaskMethod]
        public void GetLog(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                category.GetLog(context.Authentication);
            });
        }

        [TaskMethod]
        public void Find(ITableCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var text = RandomUtility.NextWord();
                var option = RandomUtility.NextEnum<FindOptions>();
                category.Find(context.Authentication, text, option);
            });
        }
    }
}
