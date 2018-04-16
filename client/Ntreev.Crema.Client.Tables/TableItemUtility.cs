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
    public static class TableItemUtility
    {
        public static bool CanFind(Authentication authentication, ITableItemDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Guest;
            return false;
        }

        public static bool CanViewLog(Authentication authentication, ITableItemDescriptor descriptor)
        {
            if (descriptor is IPermissionDescriptor permissionDescriptor)
                return permissionDescriptor.AccessType >= AccessType.Guest;
            return false;
        }

        public static async Task<bool> FindAsync(Authentication authentication, ITableItemDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITableItem tableItem && tableItem.GetService(typeof(TableDocumentServiceViewModel)) is TableDocumentServiceViewModel documentService)
            {
                var document = documentService.AddFinder(authentication, descriptor);
                documentService.SelectedDocument = document;
                return await Task.Run(() => true);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> ViewLogAsync(Authentication authentication, ITableItemDescriptor descriptor)
        {
            return await LogViewModel.ShowDialogAsync(authentication, descriptor) != null;
        }
    }
}
