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
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using System.Collections;
using System.Collections.Generic;

namespace Ntreev.Crema.Services.Domains
{
    class DomainUserCollection : ContainerBase<Guid, DomainUser>, IDomainUserCollection
    {
        private readonly Domain domain;
        private DomainUser owner;

        public DomainUserCollection(Domain domain)
        {
            this.domain = domain;
        }

        public void Add(DomainUser domainUser)
        {
            if (domainUser.Token == Guid.Empty)
            {
                throw new ArgumentException(nameof(domainUser.Token));
            }

            this.AddBase(domainUser.Token, domainUser);
        }

        public void Remove(Guid token)
        {
            this.RemoveBase(token);
        }

        public bool Contains(Guid token)
        {
            this.Dispatcher.VerifyAccess();
            return base.ContainsKey(token);
        }

        public IDomainUser this[Guid token] => base[token];

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
                    this.OwnerToken = value.Token;
                    this.owner.IsOwner = true;
                }
            }
        }

        public Guid OwnerToken { get; private set; }

        public CremaDispatcher Dispatcher
        {
            get { return this.domain.Dispatcher; }
        }

        #region IDomainUserCollection

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
