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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Services.Users
{
    class UserAuthenticationProvider : IAuthenticationProvider
    {
        private readonly string userID;
        private string userName;
        private Authority authority;

        private AuthenticationType authenticationTypes;
        private bool dummy;

        public UserAuthenticationProvider(User user)
            : this(user, false)
        {

        }

        public UserAuthenticationProvider(User user, bool dummy)
        {
            this.userID = user.ID;
            this.userName = user.UserName;
            this.authority = user.Authority;
            this.authenticationTypes = this.GetAuthenticationTypes(user.Authority, dummy);
            user.UserInfoChanged += User_UserInfoChanged;
        }

        public UserAuthenticationProvider(string userID, string userName, AuthenticationType authenticationTypes)
        {
            this.userID = userID;
            this.userName = userName;
            this.authenticationTypes = authenticationTypes;
            this.dummy = true;
        }

        public AuthenticationType AuthenticationTypes
        {
            get { return this.authenticationTypes; }
        }

        public string ID
        {
            get { return this.userID; }
        }

        public string Name
        {
            get { return this.userName; }
        }

        public Authority Authority
        {
            get { return this.authority; }
        }

        private void User_UserInfoChanged(object sender, EventArgs e)
        {
            var user = sender as User;
            this.userName = user.UserName;
            this.authority = user.Authority;
            this.authenticationTypes = this.GetAuthenticationTypes(user.Authority, this.dummy);
        }

        private AuthenticationType GetAuthenticationTypes(Authority authority, bool dummy)
        {
            var types = dummy == true ? AuthenticationType.None : AuthenticationType.User;

            switch (authority)
            {
                case Authority.Admin:
                    types |= AuthenticationType.Administrator;
                    break;
                case Authority.Guest:
                    types |= AuthenticationType.ReadOnly;
                    break;
            }
            return types;
        }
    }
}
