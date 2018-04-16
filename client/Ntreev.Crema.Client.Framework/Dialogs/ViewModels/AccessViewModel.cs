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
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Framework.Properties;
using Ntreev.ModernUI.Framework;
using System.Windows.Threading;
using Ntreev.Crema.ServiceModel;
using System.Collections.ObjectModel;
using System.Collections;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels
{
    public class AccessViewModel : ModalDialogAppBase
    {
        private readonly IAccessible accessible;
        private readonly Dispatcher dispatcher;
        private readonly Authentication authentication;
        private readonly TreeViewItemViewModel[] userItems;
        private AccessInfo accessInfo;
        private string comment;

        private ObservableCollection<AccessItemViewModel> itemsSource = new ObservableCollection<AccessItemViewModel>();
        private AccessType accessType;
        private string path;

        private AccessViewModel(Authentication authentication, IAccessible accessible, UserCategoryTreeViewItemViewModel userItem)
        {
            this.accessible = accessible;
            if (accessible is IDispatcherObject dispatcherObject)
            {
                this.dispatcher = dispatcherObject.Dispatcher;
                this.dispatcher.VerifyAccess();
                this.accessInfo = accessible.AccessInfo;
            }
            else
            {
                this.dispatcher = base.Dispatcher;
                this.accessInfo = accessible.AccessInfo;
            }
            this.authentication = authentication;
            this.path = userItem.Path;

            this.userItems = EnumerableUtility.FamilyTree<TreeViewItemViewModel>(userItem, item => item.Items).ToArray();
            foreach (var item in this.userItems.Where(i => i is UserCategoryTreeViewItemViewModel))
            {
                item.IsExpanded = true;
            }

            foreach (var item in this.accessInfo.Members)
            {
                this.itemsSource.Add(new AccessItemViewModel(this, item.UserID, item.AccessType));
            }
            this.itemsSource.CollectionChanged += (s, e) => this.NotifyOfPropertyChange(nameof(this.CanAdd));
            this.accessType = AccessType.Master;
#if DEBUG
            this.comment = "access test";
#endif
            this.DisplayName = Resources.Title_SettingAuthority;
        }

        public static async Task<AccessViewModel> CreateInstanceAsync(Authentication authentication, IAccessibleDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IAccessible accessible)
            {
                if (accessible is IServiceProvider serviceProvider && serviceProvider.GetService(typeof(IUserContext)) is IUserContext userContext)
                {
                    var viewModel = await userContext.Dispatcher.InvokeAsync(() => new UserCategoryTreeViewItemViewModel(authentication, userContext.Root));
                    return await CreateInstanceAsync(authentication, descriptor, viewModel);
                }
                throw new NotImplementedException();
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        public static async Task<AccessViewModel> CreateInstanceAsync(Authentication authentication, IAccessibleDescriptor descriptor, UserCategoryTreeViewItemViewModel userItem)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (userItem == null)
                throw new ArgumentNullException(nameof(userItem));

            if (descriptor.Target is IAccessible accessible)
            {
                var dispatcher = accessible is IDispatcherObject dispatcherObject ? dispatcherObject.Dispatcher : Application.Current.Dispatcher;
                return await dispatcher.InvokeAsync(() =>
                {
                    return new AccessViewModel(authentication, accessible, userItem);
                });
            }
            throw new NotImplementedException();
        }

        public async void Add()
        {
            try
            {
                var memberID = this.Path;
                var accessType = this.AccessType;
                await this.dispatcher.InvokeAsync(() => this.accessible.AddAccessMember(this.authentication, memberID, accessType));
                this.itemsSource.Add(new AccessItemViewModel(this, memberID, this.AccessType));
                this.Path = null;
                this.AccessType = AccessType.Master;
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        public string Comment
        {
            get { return this.comment ?? string.Empty; }
            set
            {
                this.comment = value;
                this.NotifyOfPropertyChange(nameof(this.CanAdd));
                this.NotifyOfPropertyChange(nameof(this.Comment));
            }
        }

        public bool CanAdd
        {
            get
            {
                if (NameValidator.VerifyItemPath(this.Path) == true)
                    return false;
                if (NameValidator.VerifyCategoryPath(this.Path) == true)
                    return false;
                if (this.Path == string.Empty)
                    return false;
                return this.itemsSource.Any(item => item.MemberID == this.Path) == false;
            }
        }

        public IEnumerable UserItems
        {
            get { return this.userItems; }
        }

        public string Path
        {
            get { return this.path ?? string.Empty; }
            set
            {
                this.path = value;
                this.NotifyOfPropertyChange(nameof(this.Path));
                this.NotifyOfPropertyChange(nameof(this.CanAdd));
            }
        }

        public ObservableCollection<AccessItemViewModel> ItemsSource
        {
            get { return this.itemsSource; }
        }

        public IEnumerable AccessTypes
        {
            get
            {
                yield return AccessType.Owner;
                yield return AccessType.Master;
                yield return AccessType.Developer;
                yield return AccessType.Editor;
                yield return AccessType.Guest;
            }
        }

        public AccessType AccessType
        {
            get { return this.accessType; }
            set
            {
                this.accessType = value;
                this.NotifyOfPropertyChange(nameof(this.AccessType));
                this.NotifyOfPropertyChange(nameof(this.CanAdd));
            }
        }

        internal void SetAccessInfo(AccessInfo accessInfo)
        {
            this.accessInfo = accessInfo;
            var query = from member in this.accessInfo.Members
                        join viewModel in this.itemsSource on member.UserID equals viewModel.MemberID
                        select new { ViewModel = viewModel, member.AccessType };

            foreach (var item in query)
            {
                var viewModel = item.ViewModel;
                var accessType = item.AccessType;
                viewModel.SetAccessType(accessType);
            }
        }

        internal IAccessible Accessible
        {
            get { return this.accessible; }
        }

        internal new Dispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        internal Authentication Authentication
        {
            get { return this.authentication; }
        }
    }
}
