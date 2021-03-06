﻿//Released under the MIT License.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly bool isUser;
        private readonly string userID;
        private readonly string userName;
        private readonly Authority authority;

        public AuthenticationProvider(IUser user)
        {
            this.isUser = true;
            this.userID = user.ID;
            this.userName = user.UserName;
            this.authority = user.Authority;
        }

        public AuthenticationProvider(string name)
        {
            this.userID = name;
            this.userName = name;
            this.authority = Authority.Guest;
        }

        internal AuthenticationType AuthenticationTypes
        {
            get
            {
                var authority = this.authority;
                if (authority == Authority.Admin)
                    return AuthenticationType.User | AuthenticationType.Administrator;
                else if (authority == Authority.Member)
                    return AuthenticationType.User;
                else if (this.isUser == false)
                    return AuthenticationType.None;
                return AuthenticationType.User | AuthenticationType.ReadOnly;
            }
        }

        public Authority Authority
        {
            get { return this.authority; }
        }

        public string ID
        {
            get { return this.userID; }
        }

        public string Name
        {
            get { return this.userName; }
        }

        #region IAuthenticationProvider

        AuthenticationType IAuthenticationProvider.AuthenticationTypes
        {
            get { return this.AuthenticationTypes; }
        }

        #endregion
    }
}
