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
    [Export(typeof(ITypeTask))]
    [TaskClass]
    public class ITypeTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var type = context.Target as IType;
            if (context.IsCompleted(type) == true)
            {
                context.Pop(type);
            }
            else if (RandomUtility.Within(75) == true)
            {
                var template = type.Dispatcher.Invoke(() => type.Template);
                context.Push(template);
            }
        }

        public Type TargetType
        {
            get { return typeof(IType); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod]
        public void GetAccessType(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                type.GetAccessType(context.Authentication);
            });
        }

        //[TaskMethod]
        //public void VerifyRead(IType type, TaskContext context)
        //{
        //    type.Dispatcher.Invoke(() =>
        //    {
        //        type.VerifyRead(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyOwner(IType type, TaskContext context)
        //{
        //    type.Dispatcher.Invoke(() =>
        //    {
        //        type.VerifyOwner(context.Authentication);
        //    });
        //}

        //[TaskMethod]
        //public void VerifyMember(IType type, TaskContext context)
        //{
        //    type.Dispatcher.Invoke(() =>
        //    {
        //        type.VerifyMember(context.Authentication);
        //    });
        //}

        [TaskMethod(Weight = 10)]
        public void Lock(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var comment = RandomUtility.NextString();
                if (Verify(comment) == false)
                    return;
                type.Lock(context.Authentication, comment);
            });

            bool Verify(string comment)
            {
                if (context.AllowException == true)
                    return true;
                if (string.IsNullOrEmpty(comment) == true)
                    return false;
                if (type.IsLocked == true)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void Unlock(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                if (Verify() == false)
                    return;
                type.Unlock(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (type.IsLocked == false)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void SetPublic(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                if (Verify() == false)
                    return;
                type.SetPublic(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (type.IsPrivate == false)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 10)]
        public void SetPrivate(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                type.SetPrivate(context.Authentication);
            });
        }

        [TaskMethod(Weight = 10)]
        public void AddAccessMember(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var userContext = type.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                var accessType = RandomUtility.NextEnum<AccessType>();
                if (Verify() == false)
                    return;
                type.AddAccessMember(context.Authentication, memberID, accessType);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (type.IsPrivate == false)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void RemoveAccessMember(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var userContext = type.GetService(typeof(IUserContext)) as IUserContext;
                var memberID = userContext.Dispatcher.Invoke(() => userContext.Select(item => item.Path).Random());
                if (Verify() == false)
                    return;
                type.RemoveAccessMember(context.Authentication, memberID);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (type.IsPrivate == false)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 25)]
        public void Rename(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var typeName = RandomUtility.NextIdentifier();
                type.Rename(context.Authentication, typeName);
            });
        }

        [TaskMethod(Weight = 25)]
        public void Move(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var categories = type.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                var categoryPath = categories.Random().Path;
                if (Verify(categoryPath) == false)
                    return;
                type.Move(context.Authentication, categoryPath);
            });

            bool Verify(string categoryPath)
            {
                if (context.AllowException == true)
                    return true;
                if (type.Category.Path == categoryPath)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 5)]
        public void Delete(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                type.Delete(context.Authentication);
            });
        }

        [TaskMethod]
        public void SetTags(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var tags = (TagInfo)TagInfoUtility.Names.Random();
                type.SetTags(context.Authentication, tags);
            });
        }

        [TaskMethod]
        public void Copy(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var categories = type.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                var categoryPath = categories.Random().Path;
                var typeName = RandomUtility.NextIdentifier();
                type.Copy(context.Authentication, typeName, categoryPath);
            });
        }

        [TaskMethod]
        public void Preview(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                type.GetDataSet(context.Authentication, null);
            });
        }

        [TaskMethod]
        public void GetLog(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                type.GetLog(context.Authentication);
            });
        }

        [TaskMethod]
        public void Find(IType type, TaskContext context)
        {
            type.Dispatcher.Invoke(() =>
            {
                var text = RandomUtility.NextWord();
                var option = RandomUtility.NextEnum<FindOptions>();
                type.Find(context.Authentication, text, option);
            });
        }
    }
}
