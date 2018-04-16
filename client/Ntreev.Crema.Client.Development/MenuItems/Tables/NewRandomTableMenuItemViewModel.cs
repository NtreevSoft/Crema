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
using Ntreev.Crema.Client.Tables;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using Ntreev.Crema.Client.Tables.Dialogs.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.Services.Random;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Random;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.Client.Development.MenuItems.Tables
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(Client.Tables.BrowserItems.ViewModels.TableCategoryTreeViewItemViewModel))]
    class NewRandomTableMenuItemViewModel : MenuItemBase
    {
        public NewRandomTableMenuItemViewModel()
        {
            this.DisplayName = "새 무작위 테이블";
        }

        protected async override void OnExecute(object parameter)
        {
            var viewModel = parameter as Client.Tables.BrowserItems.ViewModels.TableCategoryTreeViewItemViewModel;
            var category = viewModel.Target as ITableCategory;
            var authenticator = category.GetService(typeof(Authenticator)) as Authenticator;
            var browser = category.GetService(typeof(ITableBrowser)) as ITableBrowser;
            var isModified = false;

            var template = await category.Dispatcher.InvokeAsync(() => category.NewTable(authenticator));

            await template.Dispatcher.InvokeAsync(() =>
            {
                if (template.Count == 0)
                {
                    template.SetComment(authenticator, RandomUtility.NextString());
                    template.GenerateColumns(authenticator, 10);
                    isModified = true;
                    //{
                    //    var column = template.AddNew(authenticator);
                    //    column.SetIsKey(authenticator, true);
                    //    column.SetIsUnique(authenticator, true);
                    //    column.SetName(authenticator, "ID");
                    //    column.SetComment(authenticator, "아이디를 나타냅니다.");
                    //    column.SetDataType(authenticator, typeof(int).GetTypeName());
                    //    template.EndNew(authenticator, column);
                    //}
                    //{
                    //    var column = template.AddNew(authenticator);
                    //    column.SetName(authenticator, "Value");
                    //    column.SetComment(authenticator, "값을 나타냅니다.");
                    //    template.EndNew(authenticator, column);
                    //}
                }
            });

            var dialog = new NewTableViewModel(authenticator, category, template) { IsModified = isModified, };

            if (dialog.ShowDialog() != true)
                return;

            var tableViewModel = viewModel.Items.FirstOrDefault(item => item.DisplayName == dialog.TableName);
            browser.SelectedItem = tableViewModel;
        }
    }
}
