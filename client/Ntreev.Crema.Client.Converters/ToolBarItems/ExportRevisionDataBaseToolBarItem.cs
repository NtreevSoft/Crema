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

using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Crema.Client.Converters.Properties;
using Ntreev.Crema.Services;
using Ntreev.Crema.Spreadsheet;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Converters.ToolBarItems
{
    [Export(typeof(IToolBarItem))]
    [ParentType("Ntreev.Crema.Client.Base.Dialogs.ViewModels.LogViewModel, Ntreev.Crema.Client.Base, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
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
            if (parameter is ISelector selector && selector.SelectedItem is ListBoxItemViewModel viewModel)
            {
                return viewModel.Target is IDataBase;
            }
            return false;
        }

        protected async override void OnExecute(object parameter)
        {
            try
            {
                if (parameter is ISelector selector && selector.SelectedItem is ListBoxItemViewModel viewModel && viewModel.Target is IDataBase dataBase)
                {
                    if (viewModel is IInfoProvider provider)
                    {
                        var props = provider.Info;
                        var revision = (string)props["Revision"];
                        var dataBaseName = await dataBase.Dispatcher.InvokeAsync(() => dataBase.Name);
                        var dialog = new CommonSaveFileDialog();
                        dialog.Filters.Add(new CommonFileDialogFilter("excel file", "*.xlsx"));
                        dialog.DefaultFileName = $"{dataBaseName}_{revision}";
                        dialog.DefaultExtension = "xlsx";

                        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            var dataSet = await dataBase.Dispatcher.InvokeAsync(() => dataBase.GetDataSet(this.authenticator, revision));
                            var writer = new SpreadsheetWriter(dataSet);
                            writer.Write(dialog.FileName);
                            AppMessageBox.Show(Resources.Message_Exported);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }
    }
}
