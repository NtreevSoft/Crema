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
using Ntreev.Crema.Client.Differences.Dialogs.ViewModels;
using Ntreev.Crema.Client.Differences.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Tables.Dialogs.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Differences.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType("Ntreev.Crema.Client.Tables.BrowserItems.ViewModels.TableTreeViewItemViewModel, Ntreev.Crema.Client.Tables, Version=3.6.0.0, Culture=neutral, PublicKeyToken=null")]
    class CompareTableWithOtherDataBaseMenuItem : MenuItemBase
    {
        [Import]
        private ICremaHost cremaHost = null;
        [Import]
        private ICremaAppHost cremaAppHost = null;
        [Import]
        private Authenticator authenticator = null;

        public CompareTableWithOtherDataBaseMenuItem()
        {
            this.DisplayName = Resources.MenuItem_CompareWithOtherDataBase;
        }

        protected override void OnExecute(object parameter)
        {
            var tableDescriptor = parameter as ITableDescriptor;
            var table = tableDescriptor.Target;
            var tableName = tableDescriptor.TableInfo.Name;

            var dataBaseName = this.SelectDataBase();
            if (dataBaseName == null)
                return;

            var dataSet1 = this.PreviewOtherTable(dataBaseName, tableName);
            if (dataSet1 != null)
            {
                var dataSet2 = table.Dispatcher.Invoke(() => table.GetDataSet(this.authenticator, -1));
                var dataSet = new DiffDataSet(dataSet1, dataSet2)
                {
                    Header1 = $"{dataBaseName}: {tableName}",
                    Header2 = $"{this.cremaAppHost.DataBaseName}: {tableName}",
                };

                var dialog = new DiffDataTableViewModel(dataSet.Tables.First())
                {
                    DisplayName = Resources.Title_CompareWithOtherDataBase,
                };
                dialog.ShowDialog();
            }
            else
            {
                AppMessageBox.Show(string.Format(Resources.Message_TableNotFound_Format, tableName));
            }
        }

        private string SelectDataBase()
        {
            var dialog = new SelectDataBaseViewModel(this.authenticator, this.cremaAppHost, (info) => info.Name != this.cremaAppHost.DataBaseName);
            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedValue;
            }
            return null;
        }

        private CremaDataSet PreviewOtherTable(string dataBaseName, string tableName)
        {
            try
            {
                return this.cremaHost.Dispatcher.Invoke(() =>
                {
                    using (var item = UsingDataBase.Set(this.cremaHost, dataBaseName, this.authenticator))
                    {
                        var dataBase = item.DataBase;
                        if (dataBase.TableContext.Tables.Contains(tableName) == true)
                        {
                            var table2 = dataBase.TableContext.Tables[tableName];
                            return table2.GetDataSet(this.authenticator, -1);
                        }
                        return null;
                    }
                });
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                return null;
            }
        }
    }
}
