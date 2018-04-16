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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.ServiceModel.Properties;
using System.Xml;
using System.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using System.ComponentModel;
using Ntreev.Library;
using System.Text.RegularExpressions;
using System.Data;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class DomainUserBase
    {
        private DomainUserInfo domainUserInfo;
        private DomainUserState domainUserState;
        private PropertyCollection extendedProperties;

        protected DomainUserBase()
        {

        }

        public DomainUserInfo DomainUserInfo
        {
            get { return this.domainUserInfo; }
            set
            {
                this.domainUserInfo = value;
                this.OnDomainUserInfoChanged(EventArgs.Empty);
            }
        }

        public DomainUserState DomainUserState
        {
            get { return this.domainUserState; }
            set
            {
                this.domainUserState = value;
                this.OnDomainUserStateChanged(EventArgs.Empty);
            }
        }

        public bool IsBeingEdited
        {
            get { return this.DomainUserState.HasFlag(DomainUserState.IsBeingEdited); }
            set
            {
                if (value == true)
                    this.DomainUserState |= DomainUserState.IsBeingEdited;
                else
                    this.DomainUserState &= ~DomainUserState.IsBeingEdited;
            }
        }

        public bool IsModified
        {
            get { return this.DomainUserState.HasFlag(DomainUserState.IsModified); }
            set
            {
                if (value == true)
                    this.DomainUserState |= DomainUserState.IsModified;
                else
                    this.DomainUserState &= ~DomainUserState.IsModified;
            }
        }

        public bool IsOnline
        {
            get { return this.DomainUserState.HasFlag(DomainUserState.Online); }
            set
            {
                if (value == true)
                    this.DomainUserState |= DomainUserState.Online;
                else
                    this.DomainUserState &= ~DomainUserState.Online;
            }
        }

        public bool IsOwner
        {
            get { return this.DomainUserState.HasFlag(DomainUserState.IsOwner); }
            set
            {
                if (value == true)
                    this.DomainUserState |= DomainUserState.IsOwner;
                else
                    this.DomainUserState &= ~DomainUserState.IsOwner;
            }
        }

        public bool CanRead
        {
            get { return this.domainUserInfo.AccessType.HasFlag(DomainAccessType.Read); }
        }

        public bool CanWrite
        {
            get { return this.domainUserInfo.AccessType.HasFlag(DomainAccessType.ReadWrite); }
        }

        [Browsable(false)]
        public PropertyCollection ExtendedProperties
        {
            get
            {
                if (this.extendedProperties == null)
                {
                    this.extendedProperties = new PropertyCollection();
                }
                return this.extendedProperties;
            }
        }

        public event EventHandler DomainUserInfoChanged;

        public event EventHandler DomainUserStateChanged;

        protected virtual void OnDomainUserInfoChanged(EventArgs e)
        {
            this.DomainUserInfoChanged?.Invoke(this, e);
        }

        protected virtual void OnDomainUserStateChanged(EventArgs e)
        {
            this.DomainUserStateChanged?.Invoke(this, e);
        }

        protected void UpdateLocation(DomainLocationInfo locationInfo)
        {
            this.domainUserInfo.Location = locationInfo;
            this.OnDomainUserInfoChanged(EventArgs.Empty);
        }
    }
}
