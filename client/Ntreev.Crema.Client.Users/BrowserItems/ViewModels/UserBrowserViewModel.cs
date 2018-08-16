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
using Ntreev.Crema.Client.Framework;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using Ntreev.Crema.Services;
using System.Collections.Specialized;
using System.Windows.Input;
using Ntreev.Crema.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Users.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Crema.Client.Users.Properties;
using Ntreev.Library;

namespace Ntreev.Crema.Client.Users.BrowserItems.ViewModels
{
    [Export(typeof(IBrowserItem))]
    [Export(typeof(UserBrowserViewModel))]
    [ParentType("Ntreev.Crema.Client.Tables.IBrowserService, Ntreev.Crema.Client.Tables, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [ParentType("Ntreev.Crema.Client.Types.IBrowserService, Ntreev.Crema.Client.Types, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [ParentType("Ntreev.Crema.Client.Base.IBrowserService, Ntreev.Crema.Client.Base, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    class UserBrowserViewModel : TreeViewBase, IBrowserItem
    {
        private readonly ICremaAppHost cremaAppHost;

        [Import]
        private Authenticator authenticator = null;
        [ImportMany]
        private IEnumerable<IPropertyService> propertyServices = null;
        [Import]
        private Lazy<IShell> shell = null;

        private DelegateCommand deleteCommand;

        [Import]
        private IFlashService flashService = null;
        [Import]
        private ICompositionService compositionService = null;

        [ImportingConstructor]
        public UserBrowserViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
            this.deleteCommand = new DelegateCommand(this.Delete_Execute, this.Delete_CanExecute);
            this.DisplayName = Resources.Title_Users;
            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in this.propertyServices)
                {
                    this.AttachPropertyService(item);
                }
            });
        }

        public async void NotifyMessage()
        {
            var userContext = this.cremaAppHost.GetService(typeof(IUserContext)) as IUserContext;
            var dialog = await NotifyMessageViewModel.CreateInstanceAsync(this.authenticator, userContext);
            dialog?.ShowDialog();
        }

        public bool IsVisible => this.cremaAppHost.IsOpened;

        public bool IsAdmin => this.authenticator.Authority == Authority.Admin;

        public ICommand DeleteCommand => this.deleteCommand;

        protected override bool Predicate(IPropertyService propertyService)
        {
            return this.Shell.SelectedService.GetType().Assembly == propertyService.GetType().Assembly;
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.cremaAppHost.UserConfigs.Commit(this);
            this.FilterExpression = string.Empty;
            this.Items.Clear();
        }

        private async void CremaAppHost_Opened(object sender, EventArgs e)
        {
            if (this.cremaAppHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                var viewModel = await userContext.Dispatcher.InvokeAsync(() =>
                {
                    userContext.Users.MessageReceived += Users_MessageReceived;
                    return new UserCategoryTreeViewItemViewModel(this.authenticator, userContext.Root, this);
                });

                this.compositionService.SatisfyImportsOnce(viewModel);
                this.Items.Add(viewModel);

                this.cremaAppHost.UserConfigs.Update(this);
            }
        }

        private async void Users_MessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var messageType = e.MessageType;
            var sendUserID = e.UserID;
            var userIDs = e.Items.Select(item => item.ID).ToArray();

            if (messageType == MessageType.Notification)
            {
                if (e.UserID != this.authenticator.ID && (userIDs.Any() == false || userIDs.Any(item => item == this.authenticator.ID) == true))
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        var title = string.Format(Resources.Title_AdministratorMessage_Format, sendUserID);
                        this.flashService?.Flash();
                        AppMessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
            }
            else if (messageType == MessageType.None)
            {
                await this.Dispatcher.InvokeAsync(async () =>
                {
                    var userContext = this.cremaAppHost.GetService(typeof(IUserContext)) as IUserContext;
                    var dialog = await ViewMessageViewModel.CreateInstanceAsync(this.authenticator, userContext, message, sendUserID);
                    dialog?.ShowDialog();
                });
            }
        }

        private void Delete_Execute(object parameter)
        {
            if (parameter is UserTreeViewItemViewModel userViewModel)
            {
                userViewModel.DeleteCommand.Execute(parameter);
            }
            else if (parameter is UserCategoryTreeViewItemViewModel categoryViewModel)
            {
                categoryViewModel.DeleteCommand.Execute(parameter);
            }
        }

        private bool Delete_CanExecute(object parameter)
        {
            if (parameter is UserTreeViewItemViewModel userViewModel)
            {
                return userViewModel.DeleteCommand.CanExecute(parameter);
            }
            else if (parameter is UserCategoryTreeViewItemViewModel categoryViewModel)
            {
                return categoryViewModel.DeleteCommand.CanExecute(parameter);
            }
            return false;
        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        private string[] Settings
        {
            get { return this.GetSettings(); }
            set
            {
                this.SetSettings(value);
            }
        }

        private IShell Shell => this.shell.Value;
    }
}
