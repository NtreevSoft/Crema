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

using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using System.Windows;
using Ntreev.ModernUI.Framework;
using System.Windows.Media;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.ComponentModel.Composition;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections.ObjectModel;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Client.Framework
{
    public class DomainDescriptor : DescriptorBase, IDomainDescriptor, IPartImportsSatisfiedNotification
    {
        private IDomain domain;
        private readonly object owner;
        private readonly ObservableCollection<DomainUserDescriptor> domainUsers = new ObservableCollection<DomainUserDescriptor>();
        private readonly ReadOnlyObservableCollection<DomainUserDescriptor> domainUsersReadonly;

        private DomainInfo domainInfo;
        private DomainState domainState;

        [Import]
        private ICompositionService compositionService = null;

        public DomainDescriptor(Authentication authentication, IDomainDescriptor descriptor, bool isSubscriptable, object owner)
            : base(authentication, descriptor.Target, descriptor, isSubscriptable)
        {
            this.domain = descriptor.Target;
            this.owner = owner ?? this;
        }

        public DomainDescriptor(Authentication authentication, IDomain domain, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, domain, descriptorTypes)
        {
            this.domain = domain;
            this.owner = owner ?? this;
            this.domain.Dispatcher.VerifyAccess();
            this.domainInfo = domain.DomainInfo;
            this.domainState = domain.DomainState;

            this.domainUsersReadonly = new ReadOnlyObservableCollection<DomainUserDescriptor>(this.domainUsers);

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.domain.Deleted += Domain_Deleted;
                this.domain.RowAdded += Domain_RowAdded;
                this.domain.RowChanged += Domain_RowChanged;
                this.domain.RowRemoved += Domain_RowRemoved;
                this.domain.PropertyChanged += Domain_PropertyChanged;
                this.domain.DomainInfoChanged += Domain_DomainInfoChanged;
                this.domain.DomainStateChanged += Domain_DomainStateChanged;
            }

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
            {
                if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                {
                    this.domain.UserAdded += Domain_UserAdded;
                    this.domain.UserRemoved += Domain_UserRemoved;
                }

                foreach (var item in this.domain.Users)
                {
                    this.domainUsers.Add(new DomainUserDescriptor(authentication, item, this.descriptorTypes, this.owner));
                }
            }
        }

        [DescriptorProperty]
        public string Name => $"{this.domainInfo.DomainID}";

        [DescriptorProperty]
        public Guid DomainID => this.domainInfo.DomainID;

        [DescriptorProperty]
        public string DisplayName => NameValidator.VerifyItemPath(this.domainInfo.ItemPath) ? new ItemName(this.domainInfo.ItemPath).Name : $"{this.domainInfo.DomainID}";

        [DescriptorProperty(nameof(domainInfo))]
        public DomainInfo DomainInfo => this.domainInfo;

        [DescriptorProperty(nameof(domainState))]
        public DomainState DomainState => this.domainState;

        [DescriptorProperty]
        public bool IsActivated => DomainDescriptorUtility.IsActivated(this.authentication, this);

        [DescriptorProperty]
        public bool IsModified => DomainDescriptorUtility.IsModified(this.authentication, this);

        public ReadOnlyObservableCollection<DomainUserDescriptor> Users
        {
            get { return this.domainUsersReadonly; }
        }

        protected async override void OnDisposed(EventArgs e)
        {
            if (this.referenceTarget == null && this.domain != null)
            {
                await this.domain.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.domain.Deleted -= Domain_Deleted;
                        this.domain.RowAdded -= Domain_RowAdded;
                        this.domain.RowChanged -= Domain_RowChanged;
                        this.domain.RowRemoved -= Domain_RowRemoved;
                        this.domain.PropertyChanged -= Domain_PropertyChanged;
                        this.domain.DomainInfoChanged -= Domain_DomainInfoChanged;
                        this.domain.DomainStateChanged -= Domain_DomainStateChanged;
                    }

                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
                    {
                        if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                        {
                            this.domain.UserAdded -= Domain_UserAdded;
                            this.domain.UserRemoved -= Domain_UserRemoved;
                        }
                    }
                });
            }
            base.OnDisposed(e);
        }

        private void Domain_Deleted(object sender, DomainDeletedEventArgs e)
        {
            this.domain = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDisposed(EventArgs.Empty);
            });
        }

        private void Domain_UserAdded(object sender, DomainUserEventArgs e)
        {
            var domainUser = this.domain.Users[e.DomainUserInfo.UserID];
            var viewModel = new DomainUserDescriptor(this.authentication, domainUser, this.descriptorTypes, this.owner);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.compositionService?.SatisfyImportsOnce(viewModel);
                this.domainUsers.Add(viewModel);
            });
        }

        private void Domain_UserRemoved(object sender, DomainUserEventArgs e)
        {
            var domainUserInfo = e.DomainUserInfo;

            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in this.domainUsers.ToArray())
                {
                    if (item.UserID == domainUserInfo.UserID)
                    {
                        this.domainUsers.Remove(item);
                        break;
                    }
                }
            });
        }

        private async void Domain_RowAdded(object sender, DomainRowEventArgs e)
        {
            this.domainInfo = this.domain.DomainInfo;
            this.domainState = this.domain.DomainState;
            await this.RefreshAsync();
        }

        private async void Domain_RowChanged(object sender, DomainRowEventArgs e)
        {
            this.domainInfo = this.domain.DomainInfo;
            this.domainState = this.domain.DomainState;
            await this.RefreshAsync();
        }

        private async void Domain_RowRemoved(object sender, DomainRowEventArgs e)
        {
            this.domainInfo = this.domain.DomainInfo;
            this.domainState = this.domain.DomainState;
            await this.RefreshAsync();
        }

        private async void Domain_PropertyChanged(object sender, DomainPropertyEventArgs e)
        {
            this.domainInfo = this.domain.DomainInfo;
            this.domainState = this.domain.DomainState;
            await this.RefreshAsync();
        }

        private async void Domain_DomainInfoChanged(object sender, EventArgs e)
        {
            this.domainInfo = this.domain.DomainInfo;
            await this.RefreshAsync();
        }

        private async void Domain_DomainStateChanged(object sender, EventArgs e)
        {
            this.domainState = this.domain.DomainState;
            await this.RefreshAsync();
        }

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var item in this.domainUsers)
            {
                this.compositionService.SatisfyImportsOnce(item);
            }
        }

        #endregion

        #region IDomainDescriptor

        IDomain IDomainDescriptor.Target => this.domain;

        #endregion
    }
}
