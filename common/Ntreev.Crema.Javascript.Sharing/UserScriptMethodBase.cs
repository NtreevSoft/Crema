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
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Crema.Javascript
{
    public abstract class UserScriptMethodBase : ScriptMethodBase
    {
        private readonly ICremaHost cremaHost;

        protected UserScriptMethodBase(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        protected UserScriptMethodBase(ICremaHost cremaHost, string name)
            : base(name)
        {
            this.cremaHost = cremaHost;
        }

        protected IUser GetUser(string userID)
        {
            if (userID == null)
                throw new ArgumentNullException(nameof(userID));

            if (this.CremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                var user = userContext.Dispatcher.Invoke(() => userContext.Users[userID]);
                if (user == null)
                    throw new UserNotFoundException(userID);
                return user;
            }
            throw new NotImplementedException();
        }

        protected IUserItem GetUserItem(string userItemPath)
        {
            if (userItemPath == null)
                throw new ArgumentNullException(nameof(userItemPath));

            if (this.CremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                var userItem = userContext.Dispatcher.Invoke(() => userContext[userItemPath]);
                if (userItem == null)
                    throw new ItemNotFoundException(userItemPath);
                return userItem;
            }
            throw new NotImplementedException();
        }

        protected ICremaHost CremaHost => this.cremaHost;
    }
}
