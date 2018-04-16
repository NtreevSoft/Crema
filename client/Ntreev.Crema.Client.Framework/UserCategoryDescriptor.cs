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
using System.Collections.ObjectModel;

namespace Ntreev.Crema.Client.Framework
{
    public class UserCategoryDescriptor : DescriptorBase, IUserCategoryDescriptor, IUserItemDescriptor
    {
        private IUserCategory category;
        private readonly object owner;
        private readonly ObservableCollection<UserCategoryDescriptor> categories = new ObservableCollection<UserCategoryDescriptor>();
        private readonly ReadOnlyObservableCollection<UserCategoryDescriptor> categoriesReadonly;
        private readonly ObservableCollection<UserDescriptor> users = new ObservableCollection<UserDescriptor>();
        private readonly ReadOnlyObservableCollection<UserDescriptor> usersReadonly;

        private string categoryName;
        private string categoryPath;

        public UserCategoryDescriptor(Authentication authentication, IUserCategoryDescriptor descriptor, bool isSubscriptable, object owner)
            : base(authentication, descriptor.Target, descriptor, isSubscriptable)
        {
            this.category = descriptor.Target;
            this.owner = owner ?? this;
            this.usersReadonly = new ReadOnlyObservableCollection<UserDescriptor>(this.users);
            this.categoriesReadonly = new ReadOnlyObservableCollection<UserCategoryDescriptor>(this.categories);
        }

        public UserCategoryDescriptor(Authentication authentication, IUserCategory category, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, category, descriptorTypes)
        {
            this.category = category;
            this.owner = owner ?? this;
            this.category.Dispatcher.VerifyAccess();
            this.categoryName = category.Name;
            this.categoryPath = category.Path;

            this.usersReadonly = new ReadOnlyObservableCollection<UserDescriptor>(this.users);
            this.categoriesReadonly = new ReadOnlyObservableCollection<UserCategoryDescriptor>(this.categories);

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.category.Renamed += Category_Renamed;
                this.category.Moved += Category_Moved;
                this.category.Deleted += Category_Deleted;
            }

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
            {
                if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                {
                    this.category.Users.CollectionChanged += Users_CollectionChanged;
                    this.category.Categories.CollectionChanged += Categories_CollectionChanged;
                }

                foreach (var item in this.category.Categories)
                {
                    var descriptor = new UserCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    item.ExtendedProperties[this.owner] = descriptor;
                    this.categories.Add(descriptor);
                }

                foreach (var item in this.category.Users)
                {
                    var descriptor = new UserDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    item.ExtendedProperties[this.owner] = descriptor;
                    this.users.Add(descriptor);
                }
            }
        }

        [DescriptorProperty(nameof(categoryName))]
        public string Name => this.categoryName;

        [DescriptorProperty(nameof(categoryPath))]
        public string Path => this.categoryPath;

        [DescriptorProperty]
        public string DisplayName => this.categoryName;

        public ReadOnlyObservableCollection<UserCategoryDescriptor> Categories => this.categoriesReadonly;

        public ReadOnlyObservableCollection<UserDescriptor> Users => this.usersReadonly;

        protected async override void OnDisposed(EventArgs e)
        {
            if (this.referenceTarget == null && this.category != null)
            {
                await this.category.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.category.Renamed -= Category_Renamed;
                        this.category.Moved -= Category_Moved;
                        this.category.Deleted -= Category_Deleted;
                    }
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
                    {
                        if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                        {
                            this.category.Users.CollectionChanged -= Users_CollectionChanged;
                            this.category.Categories.CollectionChanged -= Categories_CollectionChanged;
                        }
                    }
                });
            }
            base.OnDisposed(e);
        }

        private async void Category_Renamed(object sender, EventArgs e)
        {
            this.categoryName = this.category.Name;
            this.categoryPath = this.category.Path;
            await this.RefreshAsync();
        }

        private async void Category_Moved(object sender, EventArgs e)
        {
            this.categoryName = this.category.Name;
            this.categoryPath = this.category.Path;
            await this.RefreshAsync();
        }

        private void Category_Deleted(object sender, EventArgs e)
        {
            this.category = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDisposed(EventArgs.Empty);
            });
        }

        private void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<UserDescriptor>(e.NewItems.Count);
                        foreach (IUser item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as UserDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new UserDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                item.ExtendedProperties[this.owner] = descriptor;
                                descriptorList.Add(descriptor);
                            }

                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.users.Add(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var descriptorList = new List<UserDescriptor>(e.OldItems.Count);
                        foreach (IUser item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as UserDescriptor;
                            descriptorList.Add(descriptor);
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.users.Remove(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {

                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            this.categories.Clear();
                            this.users.Clear();
                        });
                    }
                    break;
            }
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<UserCategoryDescriptor>(e.NewItems.Count);
                        foreach (IUserCategory item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as UserCategoryDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new UserCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                item.ExtendedProperties[this.owner] = descriptor;
                                descriptorList.Add(descriptor);
                            }
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.categories.Add(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var descriptorList = new List<UserCategoryDescriptor>(e.OldItems.Count);
                        foreach (IUserCategory item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as UserCategoryDescriptor;
                            descriptorList.Add(descriptor);
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.categories.Remove(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {

                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            this.categories.Clear();
                        });
                    }
                    break;
            }
        }

        #region IUserCategoryDescriptor

        IUserCategory IUserCategoryDescriptor.Target => this.category;

        #endregion

        #region IUserItemDescriptor

        IUserItem IUserItemDescriptor.Target => this.category as IUserItem;

        #endregion
    }
}
