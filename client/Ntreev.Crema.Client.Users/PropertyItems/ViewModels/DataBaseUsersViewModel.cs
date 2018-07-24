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

using Ntreev.Crema.Client.Users.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Users.Dialogs.ViewModels;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Users.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [RequiredAuthority(Authority.Guest)]
    [ParentType("Ntreev.Crema.Client.Base.IPropertyService, Ntreev.Crema.Client.Base, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    class DataBaseUsersViewModel : PropertyItemBase
    {
        private readonly ObservableCollection<DataBaseUserItemViewModel> users = new ObservableCollection<DataBaseUserItemViewModel>();
        private DataBaseUserItemViewModel selectedUser;
        private IDataBaseDescriptor descriptor;
        private INotifyPropertyChanged notifyObject;

        [Import]
        private ICompositionService compositionService = null;

        [Import]
        private Authenticator authenticator = null;
        [Import]
        private ICremaAppHost cremaAppHost = null;

        public DataBaseUsersViewModel()
        {
            this.DisplayName = Resources.Title_DataBaseUsers;
        }

        public override bool CanSupport(object obj)
        {
            return obj is IDataBaseDescriptor;
        }

        public override void SelectObject(object obj)
        {
            if (this.notifyObject != null)
            {
                this.notifyObject.PropertyChanged -= NotifyObject_PropertyChanged;
            }

            this.descriptor = obj as IDataBaseDescriptor;
            this.notifyObject = obj as INotifyPropertyChanged;
            this.SyncUsers(this.descriptor == null ? new AuthenticationInfo[] { } : this.descriptor.AuthenticationInfos);

            if (this.notifyObject != null)
            {
                this.notifyObject.PropertyChanged += NotifyObject_PropertyChanged;
            }
        }

        public async Task NotifyMessageAsync()
        {
            var userIDs = this.users.Select(item => item.ID).ToArray();
            var dialog = await NotifyMessageViewModel.CreateInstanceAsync(this.authenticator, this.cremaAppHost, userIDs);
            dialog.ShowDialog();
        }

        private void NotifyObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.descriptor.AuthenticationInfos) || e.PropertyName == string.Empty)
            {
                this.SyncUsers(this.descriptor.AuthenticationInfos);
            }
        }

        public override bool IsVisible => this.descriptor != null && this.users.Any();

        public override object SelectedObject => this.descriptor;

        public bool CanNotifyMessage
        {
            get { return this.authenticator.Authority == Authority.Admin; }
        }

        public ObservableCollection<DataBaseUserItemViewModel> Users => this.users;

        public DataBaseUserItemViewModel SelectedUser
        {
            get { return this.selectedUser; }
            set
            {
                this.selectedUser = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedUser));
            }
        }

        private void SyncUsers(AuthenticationInfo[] authenticationInfos)
        {
            foreach (var item in this.users.ToArray())
            {
                if (authenticationInfos.Any(i => i.ID == item.ID) == false)
                {
                    this.users.Remove(item);
                }
            }

            foreach (var item in authenticationInfos)
            {
                if (this.users.Any(i => i.ID == item.ID) == false)
                {
                    var viewModel = new DataBaseUserItemViewModel(item);
                    this.compositionService?.SatisfyImportsOnce(viewModel);
                    this.users.Add(viewModel);
                }
            }

            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }
    }
}
