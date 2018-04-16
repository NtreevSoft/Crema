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
using System.Security;
using System.Threading.Tasks;
using Ntreev.Library.Linq;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(IUserCategoryTask))]
    [TaskClass(Weight = 10)]
    public class IUserCategoryTask : ITaskProvider
    {
        private readonly ICremaHost cremaHost;
        private IUserContext userContext;

        [ImportingConstructor]
        public IUserCategoryTask(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closed += CremaHost_Closed;
        }

        public void InvokeTask(TaskContext context)
        {
            var category = context.Target as IUserCategory;
            if (context.IsCompleted(category) == true)
            {
                context.Pop(category);
            }
        }

        public Type TargetType
        {
            get { return typeof(IUserCategory); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod]
        public void Rename(IUserCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var categoryName = RandomUtility.NextIdentifier();
                category.Rename(context.Authentication, categoryName);
            });
        }

        [TaskMethod]
        public void Move(IUserCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                var categories = category.GetService(typeof(IUserCategoryCollection)) as IUserCategoryCollection;
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
                if (category.Path == categoryPath)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void Delete(IUserCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                if (category.Parent == null)
                    return;
                if (Verify() == false)
                    return;
                category.Delete(context.Authentication);
                context.Complete(category);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (category.Parent == null)
                    return false;
                if (EnumerableUtility.Descendants<IUserItem, IUser>(category as IUserItem, item => item.Childs).Any() == true)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 10)]
        public void AddNewCategory(IUserCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var categoryName = RandomUtility.NextIdentifier();
                category.AddNewCategory(context.Authentication, categoryName);
            });
        }

        [TaskMethod]
        public void AddNewUser(IUserCategory category, TaskContext context)
        {
            category.Dispatcher.Invoke(() =>
            {
                var index = RandomUtility.Next(int.MaxValue);
                var authority = RandomUtility.NextEnum<Authority>();
                var userID = $"{authority.ToString().ToLower()}_bot_{index}";
                var userName = "봇" + index;
                category.AddNewUser(context.Authentication, userID, ToSecureString("1111"), userName, authority);
            });
        }

        private static SecureString ToSecureString(string value)
        {
            var secureString = new SecureString();
            foreach (var item in value)
            {
                secureString.AppendChar(item);
            }
            return secureString;
        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            this.userContext = this.cremaHost.GetService(typeof(IUserContext)) as IUserContext;
        }

        private void CremaHost_Closed(object sender, EventArgs e)
        {
            this.userContext = null;
        }
    }
}
