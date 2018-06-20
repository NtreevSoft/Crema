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

using Ntreev.Crema.Client.Differences.Dialogs.ViewModels;
using Ntreev.Crema.Client.Differences.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Types.Dialogs.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.ServiceModel;
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
    [ParentType(typeof(LogInfoViewModel))]
    [DefaultMenu]
    class LogInfoViewModelTypeChangesWithPrevRevisionMenuItem : MenuItemBase
    {
        [Import]
        private Authenticator authenticator = null;
       
        public LogInfoViewModelTypeChangesWithPrevRevisionMenuItem()
        {
            this.DisplayName = Resources.MenuItem_CompareWithPreviousResivision;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (parameter is LogInfoViewModel viewModel && viewModel.Target is IType)
                return true;
            return false;
        }

        protected override void OnExecute(object parameter)
        {
            var viewModel = parameter as LogInfoViewModel;
            var type = viewModel.Target as IType;
            var dialog = new DiffDataTypeViewModel(this.Initialize(viewModel, type))
            {
                DisplayName = Resources.Title_CompareWithPreviousResivision,
            };
            dialog.ShowDialog();
        }

        private Task<DiffDataType> Initialize(LogInfoViewModel viewModel, IType type)
        {
            return type.Dispatcher.InvokeAsync(() =>
            {
                var logs = type.GetLog(this.authenticator);
                var prevLog = this.GetPrevLog(logs, viewModel.Revision);

                var header1 = prevLog != null ? $"[{prevLog.Value.DateTime}] {prevLog.Value.Revision}" : string.Empty;
                var header2 = $"[{viewModel.DateTime}] {viewModel.Revision}";
                var dataSet1 = prevLog != null ? type.GetDataSet(this.authenticator, prevLog.Value.Revision) : new CremaDataSet();
                var dataSet2 = type.GetDataSet(this.authenticator, viewModel.Revision);
                var dataSet = new DiffDataSet(dataSet1, dataSet2)
                {
                    Header1 = header1,
                    Header2 = header2,
                };
                return dataSet.Types.First();
            });
        }

        private LogInfo? GetPrevLog(LogInfo[] logs, string revision)
        {
            for (var i = 0; i < logs.Length; i++)
            {
                var item = logs[i];
                if (item.Revision == revision && i + 1 < logs.Length)
                {
                    return logs[i + 1];
                }
            }
            return null;
        }
    }
}
