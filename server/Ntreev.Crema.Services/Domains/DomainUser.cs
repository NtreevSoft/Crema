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
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Domains
{
    class DomainUser : DomainUserBase, IDomainUser
    {
        private readonly Domain domain;
        private Authentication authentication;

        public DomainUser(Domain domain, string userID, string name, DomainAccessType accessType)
        {
            this.domain = domain;
            base.DomainUserInfo = new DomainUserInfo()
            {
                UserID = userID,
                UserName = name,
                AccessType = accessType,
                Location = DomainLocationInfo.Empty,
            };
        }

        public override string ToString()
        {
            return base.DomainUserInfo.UserID;
        }

        public void BeginEdit(Authentication authentication, DomainLocationInfo location)
        {
            this.domain.BeginUserEdit(authentication, location);
        }

        public void EndEdit(Authentication authentication)
        {
            this.domain.EndUserEdit(authentication);
        }

        public void SetLocation(Authentication authentication, DomainLocationInfo location)
        {
            this.domain.SetUserLocation(authentication, location);
        }

        public void Kick(Authentication authentication, string comment)
        {
            this.domain.Kick(authentication, base.DomainUserInfo.UserID, comment);
        }

        public void SetOwner(Authentication authentication)
        {
            this.domain.SetOwner(authentication, base.DomainUserInfo.UserID);
        }

        public DomainUserMetaData GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();

            var metaData = new DomainUserMetaData()
            {
                DomainUserInfo = base.DomainUserInfo,
                DomainUserState = base.DomainUserState,
            };

            return metaData;
        }

        public new DomainUserInfo DomainUserInfo
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.DomainUserInfo;
            }
        }

        public new DomainUserState DomainUserState
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.DomainUserState;
            }
        }

        public string ID
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.DomainUserInfo.UserID;
            }
        }

        public DomainLocationInfo Location
        {
            get { return base.DomainUserInfo.Location; }
            set
            {
                base.UpdateLocation(value);
            }
        }

        

        public Authentication Authentication
        {
            get { return this.authentication; }
            set
            {
                if (this.authentication != null && value != null)
                    throw new CremaException();
                if (this.authentication == null && value == null)
                    throw new CremaException();
                this.authentication = value;
            }
        }

        public CremaDispatcher Dispatcher => this.domain.Dispatcher;

        public new event EventHandler DomainUserInfoChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.DomainUserInfoChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.DomainUserInfoChanged -= value;
            }
        }

        public new event EventHandler DomainUserStateChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.DomainUserStateChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.DomainUserStateChanged -= value;
            }
        }

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            if (serviceType == typeof(IDomain))
                return this.domain;
            return (this.domain as IServiceProvider).GetService(serviceType);
        }

        #endregion
    }
}
