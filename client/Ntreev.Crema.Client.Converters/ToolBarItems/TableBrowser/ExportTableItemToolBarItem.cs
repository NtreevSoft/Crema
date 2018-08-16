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

using Ntreev.Crema.Client.Converters.Dialogs.ViewModels;
using Ntreev.Crema.Client.Converters.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Converters.ToolBarItems.TableBrowser
{
    [Export(typeof(IToolBarItem))]
    [ParentType("Ntreev.Crema.Client.Tables.ITableBrowser, Ntreev.Crema.Client.Tables, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    class ExportRevisionDataBaseToolBarItem : ToolBarItemBase
    {
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public ExportRevisionDataBaseToolBarItem()
        {
            this.Icon = "/Ntreev.Crema.Client.Converters;component/Images/spreadsheet.png";
            this.DisplayName = Resources.MenuItem_Export;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (parameter is ISelector selector)
            {
                return selector.SelectedItem is ITableCategoryDescriptor || selector.SelectedItem is ITableDescriptor;
            }
            return false;
        }

        protected async override void OnExecute(object parameter)
        {
            if (parameter is ISelector selector)
            {
                if (selector.SelectedItem is ITableCategoryDescriptor categoryDescriptor)
                {
                    var dialog = await ExportViewModel.CreateInstanceAsync(this.authenticator, categoryDescriptor);
                    dialog?.ShowDialog();
                }
                else if (selector.SelectedItem is ITableDescriptor tableDescriptor)
                {
                    var dialog = await ExportViewModel.CreateInstanceAsync(this.authenticator, tableDescriptor);
                    dialog?.ShowDialog();
                }
            }
        }
    }
}
