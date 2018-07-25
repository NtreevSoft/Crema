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

using Ntreev.Crema.Client.Base.Services.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Users.Properties;

namespace Ntreev.Crema.Client.Users.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [RequiredAuthority(Authority.Guest)]
    [ParentType("Ntreev.Crema.Client.Base.IPropertyService, Ntreev.Crema.Client.Base, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    class DomainsViewModel : PropertyItemBase
    {
        private readonly ICremaAppHost cremaAppHost;
        private readonly ObservableCollection<DomainTreeItemBase> domains = new ObservableCollection<DomainTreeItemBase>();
        private readonly ReadOnlyObservableCollection<DomainTreeItemBase> domainsReadOnly;
        private DomainTreeItemBase selectedDomain;
        private IUserDescriptor descriptor;

        [Import]
        private ICompositionService compositionService = null;
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public DomainsViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
            this.domainsReadOnly = new ReadOnlyObservableCollection<DomainTreeItemBase>(this.domains);
            this.DisplayName = Resources.Title_UserDomainList;
        }

        public override bool CanSupport(object obj)
        {
            return obj is IUserDescriptor;
        }

        public override void SelectObject(object obj)
        {
            this.descriptor = obj as IUserDescriptor;
            if (this.descriptor != null)
            {
                foreach (var item in this.domains)
                {
                    item.IsVisible = item.ContainsUser(this.descriptor.UserID);
                }
                this.DisplayName = $"{Resources.Title_UserDomainList} [{this.descriptor.UserID}]";
            }
            else
            {
                foreach (var item in this.domains)
                {
                    item.IsVisible = false;
                }
                this.DisplayName = Resources.Title_UserDomainList;
            }
            this.SelectedDomain = null;
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        public override bool IsVisible
        {
            get
            {
                if (this.descriptor == null)
                    return false;
                foreach (var item in this.domains)
                {
                    if (item.IsVisible == true)
                        return true;
                }
                return false;
            }
        }

        public override object SelectedObject => this.descriptor;

        public ReadOnlyObservableCollection<DomainTreeItemBase> Domains => this.domainsReadOnly;

        public DomainTreeItemBase SelectedDomain
        {
            get => this.selectedDomain;
            set
            {
                this.selectedDomain = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedDomain));
            }
        }

        private async void CremaAppHost_Opened(object sender, EventArgs e)
        {
            if (this.cremaAppHost.GetService(typeof(IDomainCollection)) is IDomainCollection domainCollection)
            {
                var items = await domainCollection.Dispatcher.InvokeAsync(() =>
                {
                    domainCollection.DomainCreated += Domains_DomainCreated;
                    domainCollection.DomainDeleted += Domains_DomainDeleted;
                    var domains = domainCollection.ToArray();
                    var itemList = new List<DomainTreeItemBase>(domains.Length);
                    foreach (var item in domains)
                    {
                        itemList.Add(item.Dispatcher.Invoke(() => new DomainTreeItemBase(this.authenticator, item, true, this)));
                    }
                    return itemList.ToArray();
                });

                foreach (var item in items)
                {
                    this.compositionService?.SatisfyImportsOnce(item);
                    this.domains.Add(item);
                }
            }
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {

        }

        private void Domains_DomainDeleted(object sender, DomainDeletedEventArgs e)
        {
            var domainInfo = e.DomainInfo;
            this.Dispatcher.InvokeAsync(() =>
            {
                for (var i = 0; i < this.domains.Count; i++)
                {
                    if (this.domains[i].DomainID == domainInfo.DomainID)
                    {
                        this.domains.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        private void Domains_DomainCreated(object sender, DomainEventArgs e)
        {
            if (sender is IDomainCollection domainCollection)
            {
                var domain = e.Domain;
                if (domain.Dispatcher == null)
                    return;
                var viewModel = domain.Dispatcher.Invoke(() => new DomainTreeItemBase(this.authenticator, domain, true, this));
                this.Dispatcher.InvokeAsync(() =>
                {
                    this.compositionService?.SatisfyImportsOnce(viewModel);
                    this.domains.Add(viewModel);
                });
            }
        }
    }
}
