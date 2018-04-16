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
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Framework
{
    public class DomainTreeItemBase : DescriptorTreeItemBase<DomainDescriptor>, IDomainDescriptor
    {
        public DomainTreeItemBase(Authentication authentication, IDomain domain, bool IsSubscriptable, object owner)
            : base(authentication, new DomainDescriptor(authentication, domain, IsSubscriptable == true ? DescriptorTypes.All : DescriptorTypes.IsRecursive, owner), owner)
        {
            foreach (var item in this.descriptor.Users)
            {
                this.AddDescriptor(item);
            }

            if (this.descriptor.Users is INotifyCollectionChanged users)
            {
                users.CollectionChanged += Users_CollectionChanged;
            }
        }

        internal protected DomainTreeItemBase(Authentication authentication, DomainDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            foreach (var item in this.descriptor.Users)
            {
                this.AddDescriptor(item);
            }

            if (this.descriptor.Users is INotifyCollectionChanged users)
            {
                users.CollectionChanged += Users_CollectionChanged;
            }
        }

        public bool ContainsUser(string userID)
        {
            foreach (var item in this.Items)
            {
                if (item is DomainUserTreeItemBase viewModel && viewModel.UserID == userID)
                {
                    return true;
                }
            }
            return false;
        }

        public Guid DomainID => this.descriptor.DomainID;

        public override string DisplayName => this.descriptor.DisplayName;

        public DomainInfo DomainInfo => this.descriptor.DomainInfo;

        public DomainState DomainState => this.descriptor.DomainState;

        public bool IsActivated => this.descriptor.IsActivated;

        public bool IsModified => this.descriptor.IsModified;

        protected virtual DomainUserTreeItemBase CreateInstance(Authentication authentication, DomainUserDescriptor descriptor, object owner)
        {
            return new DomainUserTreeItemBase(authentication, descriptor, owner);
        }

        private void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is DomainUserDescriptor descriptor)
                            {
                                this.AddDescriptor(descriptor);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is DomainUserDescriptor descriptor)
                            {
                                this.RemoveDescriptor(descriptor);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        foreach (var item in this.Items.ToArray())
                        {
                            item.Parent = null;
                        }
                    }
                    break;
            }
        }

        private void AddDescriptor(DomainUserDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as DomainUserTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(DomainUserDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is DomainUserTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        #region IDomainDescriptor

        IDomain IDomainDescriptor.Target => this.descriptor.Target as IDomain;

        #endregion
    }
}
