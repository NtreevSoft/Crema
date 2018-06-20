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
using Ntreev.Library.ObjectModel;
using System.Collections;
using System.Collections.Generic;

namespace Ntreev.Crema.Services.Domains
{
    class DomainUserCollection : ContainerBase<DomainUser>, IDomainUserCollection
    {
        private readonly Domain domain;
        private DomainUser owner;

        public DomainUserCollection(Domain domain)
        {
            this.domain = domain;
        }

        public void Add(DomainUser domainUser)
        {
            this.AddBase(domainUser.ID, domainUser);
        }

        public void Remove(string userID)
        {
            this.RemoveBase(userID);
        }

        public bool Contains(string userID)
        {
            this.Dispatcher.VerifyAccess();
            return base.ContainsKey(userID);
        }

        public DomainUser Owner
        {
            get { return this.owner; }
            set
            {
                if (this.owner != null)
                {
                    this.owner.IsOwner = false;
                }
                this.owner = value;
                if (this.owner != null)
                {
                    this.OwnerUserID = this.owner.ID;
                    this.owner.IsOwner = true;
                }
            }
        }

        public string OwnerUserID { get; private set; }

        public CremaDispatcher Dispatcher => this.domain.Dispatcher;

        #region IDomainUserCollection

        IDomainUser IDomainUserCollection.this[string userID]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[userID];
            }
        }

        IDomainUser IDomainUserCollection.Owner
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Owner;
            }
        }

        IEnumerator<IDomainUser> IEnumerable<IDomainUser>.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            return this.GetEnumerator();
        }

        #endregion
    }
}
