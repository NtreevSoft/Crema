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

using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class GetUserInfoMethod : ScriptMethodBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        protected override Delegate CreateDelegate()
        {
            return new Func<string, IDictionary<string, object>>(GetUserInfo);
        }

        private IDictionary<string, object> GetUserInfo(string userID)
        {
            var userContext = this.CremaHost.GetService(typeof(IUserContext)) as IUserContext;
            return userContext.Dispatcher.Invoke(() =>
            {
                var user = userContext.Users[userID];
                var userInfo = user.UserInfo;
                var props = new Dictionary<string, object>
                {
                    { nameof(userInfo.ID), userInfo.ID },
                    { nameof(userInfo.Name), userInfo.Name },
                    { nameof(userInfo.CategoryName), userInfo.CategoryName },
                    { nameof(userInfo.Authority), $"{userInfo.Authority}" },
                    { CremaSchema.Creator, userInfo.CreationInfo.ID },
                    { CremaSchema.CreatedDateTime, userInfo.CreationInfo.DateTime },
                    { CremaSchema.Modifier, userInfo.ModificationInfo.ID },
                    { CremaSchema.ModifiedDateTime, userInfo.ModificationInfo.DateTime }
                };
                return props;
            });
        }

        private ICremaHost CremaHost => this.cremaHost.Value;
    }
}
