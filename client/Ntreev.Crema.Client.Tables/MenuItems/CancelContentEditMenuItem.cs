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

using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Tables.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(TableTreeItemBase))]
    [ParentType(typeof(TableListItemBase))]
    [Category("Close")]
    class CancelContentEditMenuItem : MenuItemBase
    {
        [Import]
        private Authenticator authenticator = null;

        public CancelContentEditMenuItem()
        {
            this.DisplayName = Resources.MenuItem_CancelContentEditing;
            this.HideOnDisabled = true;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (parameter is ITableDescriptor descriptor)
            {
                if (TableDescriptorUtility.IsBeingEdited(this.authenticator, descriptor) == false)
                    return false;
                return descriptor.TableState.HasFlag(TableState.IsOwner) || this.authenticator.Authority == Authority.Admin;
            }

            return false;
        }

        protected async override void OnExecute(object parameter)
        {
            try
            {
                if (parameter is ITableDescriptor descriptor && descriptor.Target is ITable table)
                {
                    if (AppMessageBox.ShowProceed(Resources.Message_ConfirmToCancelContentEditing) == false)
                        return;

                    await table.Dispatcher.InvokeAsync(() =>
                    {
                        table.Content.CancelEdit(this.authenticator);
                    });
                }
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }
    }
}
