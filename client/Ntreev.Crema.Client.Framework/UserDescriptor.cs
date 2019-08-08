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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Ntreev.Crema.Services;
using System.Windows.Input;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Windows;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class UserDescriptor : DescriptorBase, IUserDescriptor, IUserItemDescriptor
    {
        private IUser user;
        private readonly object owner;

        private UserInfo userInfo;
        private UserState userState;
        private BanInfo banInfo = BanInfo.Empty;

        private readonly ObservableCollection<UserAuthenticationDescriptor> userAuthentications = new ObservableCollection<UserAuthenticationDescriptor>();
        private readonly ReadOnlyObservableCollection<UserAuthenticationDescriptor> userAuthenticationsReadonly;

        public ReadOnlyObservableCollection<UserAuthenticationDescriptor> UserAuthentications => this.userAuthenticationsReadonly;

        public UserDescriptor(Authentication authentication, IUserDescriptor descriptor, bool isSubscriptable, object owner)
            : base(authentication, descriptor.Target, descriptor, isSubscriptable)
        {
            this.user = descriptor.Target;
            this.owner = owner ?? this;

            this.userAuthenticationsReadonly = new ReadOnlyObservableCollection<UserAuthenticationDescriptor>(this.userAuthentications);
        }

        public UserDescriptor(Authentication authentication, IUser user, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, user, descriptorTypes)
        {
            this.user = user;
            this.owner = owner ?? this;
            this.user.Dispatcher.VerifyAccess();
            this.userInfo = this.user.UserInfo;
            this.userState = this.user.UserState;
            this.banInfo = this.user.BanInfo;

            this.userAuthenticationsReadonly = new ReadOnlyObservableCollection<UserAuthenticationDescriptor>(this.userAuthentications);

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.user.Deleted += User_Deleted;
                this.user.UserInfoChanged += User_UserInfoChanged;
                this.user.UserStateChanged += User_UserStateChanged;
                this.user.UserBanInfoChanged += User_UserBanInfoChanged;
                this.user.Authentications.CollectionChanged += Authentications_CollectionChanged;
            }

            foreach (var userAutentication in this.user.Authentications.Values)
            {
                var userAuthenticationDescriptor = new UserAuthenticationDescriptor(this.authentication, userAutentication, this.descriptorTypes, this.owner);
                userAutentication.ExtendedProperties[this.owner] = userAuthenticationDescriptor;
                this.userAuthentications.Add(userAuthenticationDescriptor);
            }
        }

        [DescriptorProperty]
        public string UserID => this.userInfo.ID;

        [DescriptorProperty]
        public string DisplayName => this.userInfo.ID + " [" + this.userInfo.Name + "]";

        [DescriptorProperty(nameof(userInfo))]
        public UserInfo UserInfo => this.userInfo;

        [DescriptorProperty(nameof(userState))]
        public UserState UserState => this.userState;

        [DescriptorProperty(nameof(banInfo))]
        public BanInfo BanInfo => this.banInfo;

        [DescriptorProperty]
        public bool IsOnline => UserDescriptorUtility.IsOnline(this.authentication, this);

        [DescriptorProperty]
        public bool IsBanned => UserDescriptorUtility.IsBanned(this.authentication, this);

        [DescriptorProperty]
        public bool IsAdmin => UserDescriptorUtility.IsAdmin(this.authentication, this);

        [DescriptorProperty]
        public bool IsMember => UserDescriptorUtility.IsMember(this.authentication, this);

        [DescriptorProperty]
        public bool IsGuest => UserDescriptorUtility.IsGuest(this.authentication, this);

        protected async override void OnDisposed(EventArgs e)
        {
            if (this.referenceTarget == null && this.user != null)
            {
                await this.user.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.user.Deleted -= User_Deleted;
                        this.user.UserInfoChanged -= User_UserInfoChanged;
                        this.user.UserStateChanged -= User_UserStateChanged;
                        this.user.UserBanInfoChanged -= User_UserBanInfoChanged;
                        this.user.Authentications.CollectionChanged -= Authentications_CollectionChanged;
                    }
                });
            }
            base.OnDisposed(e);
        }

        private void User_Deleted(object sender, EventArgs e)
        {
            this.user = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDisposed(EventArgs.Empty);
            });
        }

        private async void User_UserInfoChanged(object sender, EventArgs e)
        {
            this.userInfo = this.user.UserInfo;
            await this.RefreshAsync();
        }

        private async void User_UserStateChanged(object sender, EventArgs e)
        {
            this.userState = this.user.UserState;
            await this.RefreshAsync();
        }

        private async void User_UserBanInfoChanged(object sender, EventArgs e)
        {
            this.banInfo = this.user.BanInfo;
            await this.RefreshAsync();
        }

        private void Authentications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<UserAuthenticationDescriptor>(e.NewItems.Count);
                        foreach (IUserAuthentication item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as UserAuthenticationDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new UserAuthenticationDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                item.ExtendedProperties[this.owner] = descriptor;
                                descriptorList.Add(descriptor);
                            }

                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.userAuthentications.Add(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var descriptorList = new List<UserAuthenticationDescriptor>(e.OldItems.Count);
                        foreach (IUserAuthentication item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as UserAuthenticationDescriptor;
                            descriptorList.Add(descriptor);
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.userAuthentications.Remove(item);
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
                            this.userAuthentications.Clear();
                        });
                    }
                    break;
            }
        }

        #region IUserItemDescriptor

        IUserItem IUserItemDescriptor.Target => this.user as IUserItem;

        string IUserItemDescriptor.Name => this.userInfo.ID;

        string IUserItemDescriptor.Path => this.userInfo.CategoryPath + this.userInfo.ID;

        #endregion

        #region IUserDescriptor

        IUser IUserDescriptor.Target => this.user;

        public IUserAuthenticationCollection Authentications => this.user.Authentications;

        #endregion
    }
}
