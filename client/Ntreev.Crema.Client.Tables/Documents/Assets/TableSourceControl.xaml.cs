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

using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Tables.Documents.Assets
{
    partial class TableSourceControl : ResourceDictionary
    {
        public TableSourceControl()
        {
            this.InitializeComponent();
        }

        private void PART_Filter_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private async void PART_DataGridControl_RowDrop(object sender, ModernDragEventArgs e)
        {
            e.Handled = true;

            var data = e.Data;
            var item = e.Item;
            if (data.GetDataPresent(typeof(int)) == true && sender is Ntreev.Crema.Client.Tables.Documents.Views.TableSourceDataGridControl gridControl)
            {
                var index = (int)data.GetData(typeof(int));
                var authenticator = gridControl.Domain.GetService(typeof(Authenticator)) as Authenticator;
                try
                {
                    var domain = gridControl.Domain;
                    await domain.Dispatcher.InvokeAsync(() => domain.SetRow(authenticator, item, CremaSchema.Index, index));
                }
                catch (Exception ex)
                {
                    AppMessageBox.ShowError(ex);
                }
            }
        }
    }
}
