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
    [ParentType("Ntreev.Crema.Client.Types.BrowserItems.ViewModels.TypeTreeViewItemViewModel, Ntreev.Crema.Client.Types, Version=3.6.0.0, Culture=neutral, PublicKeyToken=null")]
    class CompareTypeWithPrevRevisionMenuItem : MenuItemBase
    {
        [Import]
        private Authenticator authenticator = null;

        public CompareTypeWithPrevRevisionMenuItem()
        {
            this.DisplayName = Resources.MenuItem_CompareWithPreviousResivision;
        }

        protected override void OnExecute(object parameter)
        {
            var typeDescriptor = parameter as ITypeDescriptor;
            var type = typeDescriptor.Target;

            var dialog = new DiffDataTypeViewModel(this.Initialize(type))
            {
                DisplayName = Resources.Title_CompareWithPreviousResivision,
            };
            dialog.ShowDialog();
        }

        private Task<DiffDataType> Initialize(IType type)
        {
            return type.Dispatcher.InvokeAsync(() =>
            {
                var logs = type.GetLog(this.authenticator);
                var hasRevision = logs.Length >= 2;
                var dataSet1 = hasRevision ? type.GetDataSet(this.authenticator, logs[1].Revision) : new CremaDataSet();
                var dataSet2 = type.GetDataSet(this.authenticator, null);
                var header1 = hasRevision ? $"[{logs[1].DateTime}] {logs[1].Revision}" : string.Empty;
                var dataSet = new DiffDataSet(dataSet1, dataSet2)
                {
                    Header1 = header1,
                    Header2 = Resources.Text_Current,
                };
                return dataSet.Types.First();
            });
        }
    }
}
