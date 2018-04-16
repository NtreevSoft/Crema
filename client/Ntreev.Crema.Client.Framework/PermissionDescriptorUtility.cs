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

using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Framework
{
    public static class PermissionDescriptorUtility
    {
        public static bool IsPrivate(Authentication authentication, IPermissionDescriptor descriptor)
        {
            return descriptor.AccessInfo.IsPrivate;
        }

        public static bool IsAccessOwner(Authentication authentication, IPermissionDescriptor descriptor)
        {
            return descriptor.AccessInfo.IsOwner(authentication.ID);
        }

        public static bool IsAccessMember(Authentication authentication, IPermissionDescriptor descriptor)
        {
            return descriptor.AccessInfo.IsMember(authentication.ID);
        }

        public static bool IsAccessInherited(Authentication authentication, IPermissionDescriptor descriptor)
        {
            return descriptor.AccessInfo.IsInherited;
        }

        public static bool CanSetPrivate(Authentication authentication, IPermissionDescriptor descriptor)
        {
            if (descriptor.AccessInfo.IsPrivate == true && descriptor.AccessInfo.IsInherited == false)
                return false;
            return descriptor.AccessType >= AccessType.Owner;
        }

        public static bool CanSetPublic(Authentication authentication, IPermissionDescriptor descriptor)
        {
            if (descriptor.AccessInfo.IsPrivate == false || descriptor.AccessInfo.IsInherited == true)
                return false;

            return descriptor.AccessType >= AccessType.Owner;
        }

        public static bool CanSetAuthority(Authentication authentication, IPermissionDescriptor descriptor)
        {
            if (descriptor.AccessInfo.IsPrivate == false || descriptor.AccessInfo.IsInherited == true)
                return false;

            return descriptor.AccessType >= AccessType.Owner;
        }

        public static bool IsLocked(Authentication authentication, IPermissionDescriptor descriptor)
        {
            return descriptor.LockInfo.IsLocked;
        }

        public static bool IsLockOwner(Authentication authentication, IPermissionDescriptor descriptor)
        {
            return descriptor.LockInfo.IsOwner(authentication.ID);
        }

        public static bool IsLockInherited(Authentication authentication, IPermissionDescriptor descriptor)
        {
            return descriptor.LockInfo.IsInherited;
        }

        public static bool CanLock(Authentication authentication, IPermissionDescriptor descriptor)
        {
            if (descriptor.LockInfo.IsLocked == true && descriptor.LockInfo.IsInherited == false)
                return false;

            return descriptor.AccessInfo.GetAccessType(authentication.ID) >= AccessType.Editor || authentication.Authority == Authority.Admin;
        }

        public static bool CanUnlock(Authentication authentication, IPermissionDescriptor descriptor)
        {
            if (descriptor.LockInfo.IsLocked == false || descriptor.LockInfo.IsInherited == true)
                return false;

            return descriptor.LockInfo.IsOwner(authentication.ID) == true;
        }

        //public static async Task<bool> SetPrivateAsync(Authentication authentication, IPermission accessible)
        //{
        //    var dispatcher = accessible is IDispatcherObject dispatcherObject ? dispatcherObject.Dispatcher : Application.Current.Dispatcher;
        //    try
        //    {
        //        await dispatcher.InvokeAsync(() => accessible.SetPrivate(authentication));
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        AppMessageBox.ShowError(e);
        //        return false;
        //    }
        //}

        //public static async Task<bool> SetPublicAsync(Authentication authentication, IPermission accessible)
        //{
        //    var dispatcher = accessible is IDispatcherObject dispatcherObject ? dispatcherObject.Dispatcher : Application.Current.Dispatcher;
        //    try
        //    {
        //        await dispatcher.InvokeAsync(() => accessible.SetPublic(authentication));
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        AppMessageBox.ShowError(e);
        //        return false;
        //    }
        //}

        //public static async Task<bool> SetAuthorityAsync(Authentication authentication, IPermission accessible)
        //{
        //    var dispatcher = accessible is IDispatcherObject dispatcherObject ? dispatcherObject.Dispatcher : Application.Current.Dispatcher;
        //    //var descriptor = await dispatcher.InvokeAsync(() => accessible.AccessInfo);
        //    //if (accessible.AccessInfo.IsPrivate == false)
        //    //{
        //    //    if (AppMessageBox.ShowProceed(Resources.Message_ConfirmToSetPrivate) == false)
        //    //        return;

        //    //    try
        //    //    {
        //    //        await dispatcher.InvokeAsync(() => accessible.SetPrivate(authentication));
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        AppMessageBox.ShowError(e);
        //    //        return;
        //    //    }
        //    //}

        //    if (accessible is IServiceProvider serviceProvider)
        //    {
        //        var userContext = serviceProvider.GetService(typeof(IUserContext)) as IUserContext;
        //        var viewModel = await userContext.Dispatcher.InvokeAsync(() => new UserCategoryTreeViewItemViewModel(userContext.Root));
        //        var dialog = await dispatcher.InvokeAsync(() => new AccessViewModel(accessible, viewModel, authentication));
        //        return dialog.ShowDialog() == true;
        //    }
        //    return false;
        //}
    }
}
