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
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Framework
{
    public class DomainUserListBase : PropertyChangedBase, IPartImportsSatisfiedNotification
    {
        private readonly ObservableCollection<DomainUserListItemBase> users = new ObservableCollection<DomainUserListItemBase>();
        private readonly ReadOnlyObservableCollection<DomainUserListItemBase> usersReadOnly;
        private readonly Authentication authentication;
        private readonly DomainDescriptor descriptor;
        private readonly bool IsSubscriptable;
        private readonly object owner;

        [Import]
        private ICompositionService compositionService = null;

        public DomainUserListBase(Authentication authentication, IDomain domain, bool IsSubscriptable, object owner)
        {
            this.authentication = authentication;
            this.descriptor = new DomainDescriptor(authentication, domain, IsSubscriptable == true ? DescriptorTypes.All : DescriptorTypes.IsRecursive, owner);
            this.IsSubscriptable = IsSubscriptable;
            this.owner = owner;
            this.usersReadOnly = new ReadOnlyObservableCollection<DomainUserListItemBase>(this.users);
            domain.ExtendedProperties[this.owner] = this;

            foreach (var item in this.descriptor.Users)
            {
                this.AddDescriptor(item);
            }

            if (this.descriptor.Users is INotifyCollectionChanged users)
            {
                users.CollectionChanged += Users_CollectionChanged;
            }
        }

        public ReadOnlyObservableCollection<DomainUserListItemBase> Users => this.usersReadOnly;

        protected virtual DomainUserListItemBase CreateInstance(Authentication authentication, DomainUserDescriptor descriptor, object owner)
        {
            return new DomainUserListItemBase(authentication, descriptor, this.owner);
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
                        this.users.Clear();
                    }
                    break;
            }
        }

        private void AddDescriptor(DomainUserDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.owner) : descriptor.Host as DomainUserListItemBase;
            this.compositionService?.SatisfyImportsOnce(viewModel);
            this.users.Add(viewModel);
        }

        private void RemoveDescriptor(DomainUserDescriptor descriptor)
        {
            foreach (var item in this.users.ToArray())
            {
                if (item is DomainUserListItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.users.Remove(viewModel);
                }
            }
        }

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var item in this.users)
            {
                this.compositionService.SatisfyImportsOnce(item);
            }
        }

        #endregion
    }
}
