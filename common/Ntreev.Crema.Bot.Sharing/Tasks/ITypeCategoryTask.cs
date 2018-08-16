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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Random;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(ITypeCategoryTask))]
    [TaskClass]
    public class ITypeCategoryTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var category = context.Target as ITypeCategory;
            category.Dispatcher.Invoke(() =>
            {
                var types = category.GetService(typeof(ITypeCollection)) as ITypeCollection;
                var categories = category.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                if (context.IsCompleted(category) == true)
                {
                    context.Pop(category);
                }
                else if (categories.Count < RandomUtility.Next(Math.Max(10, categories.Count + 1)))
                {
                    this.AddNewCategory(category, context);
                    context.Complete(category);
                }
                else if (types.Count < RandomUtility.Next(Math.Max(10, types.Count + 1)))
                {
                    var template = category.NewType(context.Authentication);
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
            get { return typeof(ITypeCategory); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod]
        public void GetAccessType(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                category.GetAccessType(context.Authentication);
            });
        }

        //[TaskMethod]
        //public void VerifyRead(ITypeCategory category, TaskContext context)
        //{
        //    category.Dispatcher.Invoke(() =>
        //    {
        //        category.VerifyRead(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyOwner(ITypeCategory category, TaskContext context)
        //{
        //    category.Dispatcher.Invoke(() =>
        //    {
        //        category.VerifyOwner(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyMember(ITypeCategory category, TaskContext context)
        //{
        //    category.Dispatcher.Invoke(() =>
        //    {
        //        category.VerifyMember(context.Authentication);
        //    });
        //}

        [TaskMethod(Weight = 10)]
        public void Lock(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var comment = RandomUtility.NextString();
                category.Lock(context.Authentication, comment);
            });
        }

        [TaskMethod]
        public void Unlock(ITypeCategory category, TaskContext context)
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
        public void SetPublic(ITypeCategory category, TaskContext context)
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
        public void SetPrivate(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                if (Verify() == false)
                    return;
                category.SetPrivate(context.Authentication);
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
        public void AddAccessMember(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var userContext = category.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                var accessType = RandomUtility.NextEnum<AccessType>();
                if (Verify() == false)
                    return;
                if (NameValidator.VerifyItemPath(memberID) == true)
                {
                    category.AddAccessMember(context.Authentication, new ItemName(memberID).Name, accessType);
                }
                else
                {
                    category.AddAccessMember(context.Authentication, memberID, accessType);
                }
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

        [TaskMethod]
        public void RemoveAccessMember(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var userContext = category.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                if (Verify() == false)
                    return;
                if (NameValidator.VerifyItemPath(memberID) == true)
                {
                    category.RemoveAccessMember(context.Authentication, new ItemName(memberID).Name);
                }
                else
                {
                    category.RemoveAccessMember(context.Authentication, memberID);
                }
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

        [TaskMethod(Weight = 25)]
        public void Rename(ITypeCategory category, TaskContext context)
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

        [TaskMethod(Weight = 25)]
        public void Move(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var categories = category.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                var categoryPath = categories.Random().Path;
                if (Verify(categoryPath) == false)
                    return;
                category.Move(context.Authentication, categoryPath);
            });

            bool Verify(string categoryPath)
            {
                if (context.AllowException == true)
                    return true;
                if (category.Parent == null || category.Path == categoryPath)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 1)]
        public void Delete(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
            });
        }

        [TaskMethod]
        public void AddNewCategory(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var categoryNanme = RandomUtility.NextIdentifier();
                category.AddNewCategory(context.Authentication, categoryNanme);
            });
        }

        [TaskMethod(Weight = 10)]
        public void NewType(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var template = category.NewType(context.Authentication);
                context.Push(template);
            });
        }

        [TaskMethod]
        public void Preview(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                category.GetDataSet(context.Authentication, null);
            });
        }

        [TaskMethod]
        public void GetLog(ITypeCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                category.GetLog(context.Authentication, null);
            });
        }

        [TaskMethod]
        public void Find(ITypeCategory category, TaskContext context)
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
