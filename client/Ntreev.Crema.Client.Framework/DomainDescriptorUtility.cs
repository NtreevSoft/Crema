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
    public static class DomainDescriptorUtility
    {
        public static bool IsActivated(Authentication authentication, IDomainDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return descriptor.DomainState.HasFlag(DomainState.IsActivated);
        }

        public static bool IsModified(Authentication authentication, IDomainDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return descriptor.DomainState.HasFlag(DomainState.IsModified);
        }

        public static bool CanCancelEdit(Authentication authentication, IDomainDescriptor descriptor)
        {
            return authentication.Authority == Authority.Admin;
            //if (descriptor is IPermissionDescriptor permissionDescriptor)
            //    return permissionDescriptor.AccessType >= AccessType.Developer;
            //return false;
        }

        public static bool CanEndEdit(Authentication authentication, IDomainDescriptor descriptor)
        {
            return authentication.Authority == Authority.Admin;
            //if (descriptor is IPermissionDescriptor permissionDescriptor)
            //    return permissionDescriptor.AccessType >= AccessType.Developer;
            //return false;
        }

        public static async Task<bool> CancelEditAsync(Authentication authentication, IDomainDescriptor descriptor)
        {
            if (descriptor.Target is IDomain domain)
            {
                if (AppMessageBox.ShowProceed("편집을 취소합니다. 저장되지 않는 항목은 사라집니다. 계속하시겠습니까?") == false)
                    return false;

                try
                {
                    await domain.Dispatcher.InvokeAsync(() => domain.Delete(authentication, true));
                    return true;
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> EndEditAsync(Authentication authentication, IDomainDescriptor descriptor)
        {
            if (descriptor.Target is IDomain domain)
            {
                if (AppMessageBox.ShowProceed("편집을 종료합니다. 계속하시겠습니까?") == false)
                    return false;

                try
                {
                    await domain.Dispatcher.InvokeAsync(() => domain.Delete(authentication, false));
                    return true;
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
