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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class DomainCategoryTreeItemBase : DescriptorTreeItemBase<DomainCategoryDescriptor>, IDomainCategoryDescriptor, IDomainItemDescriptor
    {
        public DomainCategoryTreeItemBase(Authentication authentication, IDomainCategory category, object owner)
            : this(authentication, new DomainCategoryDescriptor(authentication, category, DescriptorTypes.All, owner), owner)
        {

        }

        internal protected DomainCategoryTreeItemBase(Authentication authentication, DomainCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.Owner = owner;

            foreach (var item in this.descriptor.Categories)
            {
                this.AddDescriptor(item);
            }

            foreach (var item in this.descriptor.Domains)
            {
                this.AddDescriptor(item);
            }

            if (this.descriptor.Categories is INotifyCollectionChanged categories)
            {
                categories.CollectionChanged += Categories_CollectionChanged;
            }

            if (this.descriptor.Domains is INotifyCollectionChanged tables)
            {
                tables.CollectionChanged += Domains_CollectionChanged;
            }
        }

        public string Name => this.descriptor.Name;

        public string Path => this.descriptor.Path;

        public bool IsActivated => this.descriptor.IsActivated;

        public override int Order => 1;

        public override string DisplayName => this.descriptor.Name;

        protected virtual DomainTreeItemBase CreateInstance(Authentication authentication, DomainDescriptor descriptor, object owner)
        {
            return new DomainTreeItemBase(authentication, descriptor, owner);
        }

        protected virtual DomainCategoryTreeItemBase CreateInstance(Authentication authentication, DomainCategoryDescriptor descriptor, object owner)
        {
            return new DomainCategoryTreeItemBase(authentication, descriptor, owner);
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == string.Empty && this.Parent != null)
            {
                this.Parent.Items.Reposition(this);
            }
            this.NotifyOfPropertyChange(e.PropertyName);
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is DomainCategoryDescriptor descriptor)
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
                            if (item is DomainCategoryDescriptor descriptor)
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

        private void Domains_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is DomainDescriptor descriptor)
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
                            if (item is DomainDescriptor descriptor)
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

        private void AddDescriptor(DomainCategoryDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as DomainCategoryTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(DomainCategoryDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is DomainCategoryTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        private void AddDescriptor(DomainDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as DomainTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(DomainDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is DomainTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        #region IDomainCategoryDescriptor

        IDomainCategory IDomainCategoryDescriptor.Target => this.descriptor.Target as IDomainCategory;

        #endregion

        #region IDomainItemDescriptor

        IDomainItem IDomainItemDescriptor.Target => this.descriptor.Target as IDomainItem;

        #endregion
    }
}
