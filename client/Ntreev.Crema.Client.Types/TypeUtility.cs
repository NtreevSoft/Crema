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
using Ntreev.Crema.Client.Types.Dialogs.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Types
{
    public static class TypeUtility
    {
        public static bool CanRename(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanMove(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanDelete(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanNewChildType(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanEditContent(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Editor;
            return false;
        }

        public static bool CanEditTemplate(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Developer;
            return false;
        }

        public static bool CanViewTemplate(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Guest;
            return false;
        }

        public static bool CanCancelEdit(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Developer;
            return false;
        }

        public static bool CanEditEdit(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Developer;
            return false;
        }

        public static bool CanCopy(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanInherit(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanViewLog(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Guest;
            return false;
        }

        public static async Task<bool> RenameAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            var comment = await LockAsync(authentication, descriptor, nameof(IType.Rename));
            if (comment == null)
                return false;

            var dialog = await RenameTypeViewModel.CreateInstanceAsync(authentication, descriptor);
            dialog?.ShowDialog();

            await UnlockAsync(authentication, descriptor, comment);
            return dialog?.DialogResult == true;
        }

        public static async Task<bool> MoveAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            var comment = await LockAsync(authentication, descriptor, nameof(IType.Move));
            if (comment == null)
                return false;

            var dialog = await MoveTypeViewModel.CreateInstanceAsync(authentication, descriptor);
            dialog?.ShowDialog();

            await UnlockAsync(authentication, descriptor, comment);
            return dialog?.DialogResult == true;
        }

        public static async Task<bool> DeleteAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            var dialog = await DeleteTypeViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> ViewTemplateAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor.Target is IType type)
            {
                var service = type.GetService(typeof(ITypeDocumentService)) as ITypeDocumentService;
                service.OpenType(authentication, descriptor);
                return await Task.Run(() => true);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> EditTemplateAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            var comment = await LockAsync(authentication, descriptor, "EditTemplate");
            if (comment == null)
                return false;

            var dialog = await EditTemplateViewModel.CreateInstanceAsync(authentication, descriptor);
            dialog?.ShowDialog();

            await UnlockAsync(authentication, descriptor, comment);
            return dialog?.DialogResult == true;
        }

        public static async Task<string> CopyAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            var dialog = await CopyTypeViewModel.CreateInstanceAsync(authentication, descriptor);
            if (dialog?.ShowDialog() == true)
                return dialog.NewName;
            return null;
        }

        public static async Task<bool> ViewLogAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor.Target is IType type)
            {
                var dialog = new LogViewModel(authentication, type as ITypeItem);
                dialog.ShowDialog();
                return await Task.Run(() => dialog.DialogResult == true);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static async Task<string> LockAsync(Authentication authentication, ITypeDescriptor descriptor, string comment)
        {
            if (descriptor.Target is IType type)
            {
                try
                {
                    return await type.Dispatcher.InvokeAsync(() =>
                    {
                        if (type.IsLocked == false || type.LockInfo.IsInherited == true)
                        {
                            var lockComment = comment + ":" + Guid.NewGuid();
                            type.Lock(authentication, lockComment);
                            return lockComment;
                        }
                        return string.Empty;
                    });
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                    return null;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static async Task UnlockAsync(Authentication authentication, ITypeDescriptor descriptor, string comment)
        {
            if (descriptor.Target is IType type)
            {
                if (type.Dispatcher == null)
                    return;

                try
                {
                    await type.Dispatcher.InvokeAsync(() =>
                    {
                        var lockInfo = type.LockInfo;
                        if (lockInfo.IsLocked == true && lockInfo.IsInherited == false && lockInfo.Comment == comment)
                        {
                            type.Unlock(authentication);
                        }
                    });
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
