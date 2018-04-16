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
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Windows.Input;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Client.Framework
{
    public class UserCategoryTreeItemBase : DescriptorTreeItemBase<UserCategoryDescriptor>, IUserCategoryDescriptor, IUserItemDescriptor
    {
        public UserCategoryTreeItemBase(Authentication authentication, IUserCategory category, bool isSubscriptable, object owner)
            : this(authentication, new UserCategoryDescriptor(authentication, category, isSubscriptable == true ? DescriptorTypes.All : DescriptorTypes.IsRecursive, owner), owner)
        {

        }

        public UserCategoryTreeItemBase(Authentication authentication, UserCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            foreach (var item in this.descriptor.Categories)
            {
                this.AddDescriptor(item);
            }

            foreach (var item in this.descriptor.Users)
            {
                this.AddDescriptor(item);
            }

            if (this.descriptor.Categories is INotifyCollectionChanged categories)
            {
                categories.CollectionChanged += Categories_CollectionChanged;
            }

            if (this.descriptor.Users is INotifyCollectionChanged users)
            {
                users.CollectionChanged += Users_CollectionChanged;
            }
        }

        public string Name => this.descriptor.Name;

        public string Path => this.descriptor.Path;

        public override string DisplayName => this.descriptor.DisplayName;

        public override int Order => 1;
        
        protected virtual UserCategoryTreeItemBase CreateInstance(Authentication authentication, UserCategoryDescriptor descriptor, object owner)
        {
            return new UserCategoryTreeItemBase(authentication, descriptor, owner);
        }

        protected virtual UserTreeItemBase CreateInstance(Authentication authentication, UserDescriptor descriptor, object owner)
        {
            return new UserTreeItemBase(authentication, descriptor, owner);
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
                            if (item is UserCategoryDescriptor descriptor)
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
                            if (item is UserCategoryDescriptor descriptor)
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

        private void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is UserDescriptor descriptor)
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
                            if (item is UserDescriptor descriptor)
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

        private void AddDescriptor(UserCategoryDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as UserCategoryTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(UserCategoryDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is UserCategoryTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        private void AddDescriptor(UserDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as UserTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(UserDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is UserTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        #region IUserCategoryDescriptor

        IUserCategory IUserCategoryDescriptor.Target => this.descriptor.Target as IUserCategory;

        #endregion

        #region IUserItemDescriptor

        IUserItem IUserItemDescriptor.Target => this.descriptor.Target as IUserItem;

        #endregion
    }
}
