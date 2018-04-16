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

using Ntreev.Crema.Client.Base.Dialogs.ViewModels;
using Ntreev.Crema.Client.Base.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base
{
    public static class DataBaseUtility
    {
        public static bool CanCopy(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanCreate(Authentication authentication)
        {
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanRename(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (DataBaseDescriptorUtility.IsLoaded(authentication, descriptor) == true)
                return false;
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanDelete(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (DataBaseDescriptorUtility.IsLoaded(authentication, descriptor) == true)
                return false;
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanLoad(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (DataBaseDescriptorUtility.IsLoaded(authentication, descriptor) == true)
                return false;
            if (descriptor is IAccessibleDescriptor accessibleDescriptor)
                return AccessibleDescriptorUtility.IsPrivate(authentication, accessibleDescriptor) == false && authentication.Authority == Authority.Admin;
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanUnload(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (DataBaseDescriptorUtility.IsLoaded(authentication, descriptor) == false)
                return false;
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanViewLog(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Guest;
            return false;
        }

        public async static Task<bool> CreateAsync(Authentication authentication, IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(ICremaHost)) is ICremaHost cremaHost)
            {
                var dialog = await CreateDataBaseViewModel.CreateInstanceAsync(authentication, cremaHost);
                return dialog?.ShowDialog() == true;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async static Task<bool> CopyAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            var dialog = await CopyDataBaseViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public async static Task<string> RenameAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            var dialog = await RenameDataBaseViewModel.CreateInstanceAsync(authentication, descriptor);
            if (dialog?.ShowDialog() == true)
                return dialog.NewName;
            return null;
        }

        public async static Task<bool> DeleteAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (descriptor.Target is IDataBase dataBase)
            {
                if (new DeleteViewModel().ShowDialog() != true)
                    return false;

                try
                {
                    await dataBase.Dispatcher.InvokeAsync(() => dataBase.Delete(authentication));
                    AppMessageBox.Show(Resources.Message_Deleted);
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

        public async static Task<bool> LoadAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (descriptor.Target is IDataBase dataBase)
            {
                try
                {
                    await dataBase.Dispatcher.InvokeAsync(() => dataBase.Load(authentication));
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

        public async static Task<bool> UnloadAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (descriptor.Target is IDataBase dataBase)
            {
                try
                {
                    if (descriptor.AuthenticationInfos.Any() == true && AppMessageBox.ShowProceed(Resources.Message_VerifyToCloseDataBase) == false)
                        return false;
                    await dataBase.Dispatcher.InvokeAsync(() => dataBase.Unload(authentication));
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

        public static async Task<bool> ViewLogAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            return await LogViewModel.ShowDialogAsync(authentication, descriptor) != null;
        }
    }
}
