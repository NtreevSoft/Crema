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
using Ntreev.Crema.Client.Tables.Dialogs.ViewModels;
using Ntreev.Crema.Client.Tables.Documents.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Tables
{
    public static class TableUtility
    {
        public static bool CanRename(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanMove(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanDelete(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanNewTable(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanEditContent(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Editor;
            return false;
        }

        public static bool CanViewContent(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Guest;
            return false;
        }

        public static bool CanEditTemplate(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Developer;
            return false;
        }

        public static bool CanViewTemplate(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Guest;
            return false;
        }

        public static bool CanCancelEdit(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Developer;
            return false;
        }

        public static bool CanEditEdit(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Developer;
            return false;
        }

        public static bool CanCopy(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        public static bool CanInherit(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Master;
            return false;
        }

        //public static bool CanViewLog(Authentication authentication, ITableDescriptor descriptor)
        //{
        //    if (descriptor is IPermissionDescriptor permissionDescriptor)
        //        return permissionDescriptor.AccessType >= AccessType.Guest;
        //    return false;
        //}

        public static async Task<bool> RenameAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var comment = await LockAsync(authentication, descriptor, nameof(ITable.Rename));
            if (comment == null)
                return false;

            var dialog = await RenameTableViewModel.CreateInstanceAsync(authentication, descriptor);
            dialog?.ShowDialog();

            await UnlockAsync(authentication, descriptor, comment);
            return dialog?.DialogResult == true;
        }

        public static async Task<bool> MoveAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var comment = await LockAsync(authentication, descriptor, nameof(ITable.Move));
            if (comment == null)
                return false;

            var dialog = await MoveTableViewModel.CreateInstanceAsync(authentication, descriptor);
            dialog?.ShowDialog();

            await UnlockAsync(authentication, descriptor, comment);
            return dialog?.DialogResult == true;
        }

        public static async Task<bool> DeleteAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var dialog = await DeleteTableViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> EditContentAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor.Target is ITable table && table.GetService(typeof(TableDocumentServiceViewModel)) is TableDocumentServiceViewModel documentService)
            {
                var document = documentService.Find(descriptor) ?? documentService.OpenTable(authentication, descriptor);
                document.SelectedTable = documentService.FindItem(descriptor);
                documentService.SelectedDocument = document;
                return await Task.Run(() => true);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> ViewContentAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor.Target is ITable table && table.GetService(typeof(TableDocumentServiceViewModel)) is TableDocumentServiceViewModel documentService)
            {
                var document = documentService.Find(descriptor) ?? documentService.ViewTable(authentication, descriptor);
                document.SelectedTable = documentService.FindItem(descriptor);
                documentService.SelectedDocument = document;
                return await Task.Run(() => true);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> CancelEditAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor.Target is ITable table)
            {
                if (AppMessageBox.ShowProceed("편집을 취소합니다. 저장되지 않는 항목은 사라집니다. 계속하시겠습니까?") == false)
                    return false;

                try
                {
                    await table.Dispatcher.InvokeAsync(() => table.Content.CancelEdit(authentication));
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

        public static async Task<bool> EndEditAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            if (descriptor.Target is ITable table)
            {
                if (AppMessageBox.ShowProceed("편집을 종료합니다. 계속하시겠습니까?") == false)
                    return false;

                try
                {
                    await table.Dispatcher.InvokeAsync(() => table.Content.EndEdit(authentication));
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

        public static async Task<bool> EditTemplateAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var comment = await LockAsync(authentication, descriptor, nameof(EditTemplateAsync));
            if (comment == null)
                return false;

            var dialog = await EditTemplateViewModel.CreateInstanceAsync(authentication, descriptor);
            dialog?.ShowDialog();

            await UnlockAsync(authentication, descriptor, comment);
            return dialog?.DialogResult == true;
        }

        public static async Task<bool> ViewTemplateAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var dialog = await ViewTemplateViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> CopyAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var dialog = await CopyTableViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> InheritAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var dialog = await CopyTableViewModel.CreateInstanceAsync(authentication, descriptor, true);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> NewTableAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            var comment = await LockAsync(authentication, descriptor, nameof(ITable.NewTable));
            if (comment == null)
                return false;

            var dialog = await NewChildTableViewModel.CreateInstanceAsync(authentication, descriptor);
            dialog?.ShowDialog();

            await UnlockAsync(authentication, descriptor, comment);
            return dialog?.DialogResult == true;
        }

        //public static async Task<bool> ViewLogAsync(Authentication authentication, ITableDescriptor descriptor)
        //{
        //    return await LogViewModel.ShowDialogAsync(authentication, descriptor as ITableItemDescriptor) != null;
        //}

        private static async Task<string> LockAsync(Authentication authentication, ITableDescriptor descriptor, string comment)
        {
            if (descriptor.Target is ITable table)
            {
                try
                {
                    return await table.Dispatcher.InvokeAsync(() =>
                    {
                        if (table.IsLocked == false || table.LockInfo.IsInherited == true)
                        {
                            var lockComment = comment + ":" + Guid.NewGuid();
                            table.Lock(authentication, lockComment);
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

        private static async Task UnlockAsync(Authentication authentication, ITableDescriptor descriptor, string comment)
        {
            if (descriptor.Target is ITable table)
            {
                if (table.Dispatcher == null)
                    return;

                try
                {
                    await table.Dispatcher.InvokeAsync(() =>
                    {
                        var lockInfo = table.LockInfo;
                        if (lockInfo.IsLocked == true && lockInfo.IsInherited == false && lockInfo.Comment == comment)
                        {
                            table.Unlock(authentication);
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

        //private static ITableDocument Find(ITableDocumentService documentService, ITableDescriptor descriptor)
        //{
        //    foreach (var document in documentService.Documents)
        //    {
        //        if (document is ITableDocument tableDocument)
        //        {
        //            foreach (var documentItem in tableDocument.TableItems)
        //            {
        //                if (documentItem.Target == descriptor.Target)
        //                {
        //                    return tableDocument;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private static ITableDocumentItem FindItem(ITableDocumentService documentService, ITableDescriptor descriptor)
        //{
        //    foreach (var document in documentService.Documents)
        //    {
        //        if (document is ITableDocument tableDocument)
        //        {
        //            foreach (var documentItem in tableDocument.TableItems)
        //            {
        //                if (documentItem.Target == descriptor.Target)
        //                {
        //                    return documentItem;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
