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
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Differences.BrowserItems.ViewModels
{
    [Export(typeof(IBrowserItem))]
    [ParentType(typeof(BrowserService))]
    class AddDifferenceItemViewModel : ViewModelBase, IBrowserItem
    {
        [Import]
        private ICremaHost cremaHost = null;
        [Import]
        private ICremaAppHost cremaAppHost = null;

        [Import]
        private Authenticator authenticator = null;
        [Import]
        private Lazy<BrowserService> browserService = null;

        public void Add()
        {
            var destDataBaseName = this.SelectDataBase();
            if (destDataBaseName == null)
                return;

            var task = new BackgroundTask((p, c) =>
            {
                p.Report(0, "현재 데이터 베이스 가져오는중");
                var dataSet1 = this.cremaHost.Dispatcher.Invoke(() =>
                {
                    var dataBase = this.cremaHost.DataBases[this.cremaAppHost.DataBaseName];
                    return dataBase.GetDataSet(this.authenticator, -1);
                });
                p.Report(0.33, "대상 데이터 베이스 가져오는중");
                var dataSet2 = this.cremaHost.Dispatcher.Invoke(() =>
                {
                    var dataBase = this.cremaHost.DataBases[destDataBaseName];
                    if (dataBase.IsLoaded == false)
                        dataBase.Load(this.authenticator);
                    dataBase.Enter(this.authenticator);
                    try
                    {
                        return dataBase.GetDataSet(this.authenticator, -1);
                    }
                    finally
                    {
                        dataBase.Leave(this.authenticator);
                    }
                });
                p.Report(0.66, "비교하는중");
                return new DiffDataSet(dataSet2, dataSet1, DiffMergeTypes.ReadOnly2)
                {
                    Header1 = destDataBaseName,
                    Header2 = this.cremaAppHost.DataBaseName
                };
            });

            task.ProgressChanged += Task_ProgressChanged;
            var dialog = new BackgroundTaskViewModel(task) { DisplayName = "데이터 베이스 비교하기", };
            dialog.ShowDialog();
        }

        public string DisplayName => "데이터 베이스 비교하기";

        public bool IsVisible => true;

        private void Task_ProgressChanged(object sender, Library.ProgressChangedEventArgs e)
        {
            if (sender is BackgroundTask task)
            {
                if (e.State == ProgressChangeState.Completed && task.Result is DiffDataSet dataSet)
                {
                    this.browserService.Value.Add(dataSet);
                }
            }
        }

        private string SelectDataBase()
        {
            var dialog = new SelectDataBaseViewModel(this.authenticator, this.cremaHost.Address, (item) => item.Name != this.cremaAppHost.DataBaseName);
            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedValue;
            }
            return null;
        }
    }
}
