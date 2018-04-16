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

using Ntreev.Crema.Client.Differences.BrowserItems.ViewModels;
using Ntreev.Crema.Client.Differences.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Data;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Differences.PropertyItems.ViewModels
{
    abstract class TemplateColumnInfoViewModel : PropertyItemBase
    {
        private TemplateTreeViewItemViewModel viewModel;
        private TableInfo? tableInfo;
        private TemplateColumnInfoItemViewModel[] columns = new TemplateColumnInfoItemViewModel[] { };

        public TemplateColumnInfoViewModel()
        {
            this.DisplayName = Resources.Title_ColumnInfo;
        }

        public TemplateColumnInfoItemViewModel[] Columns
        {
            get { return this.columns; }
        }

        public override bool IsVisible
        {
            get { return this.columns.Any(); }
        }

        public override object SelectedObject
        {
            get { return this.viewModel; }
        }

        public override bool CanSupport(object obj)
        {
            return obj is TemplateTreeViewItemViewModel;
        }

        public override void SelectObject(object obj)
        {
            if (this.viewModel != null)
            {
                this.viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            this.viewModel = obj as TemplateTreeViewItemViewModel;
            if (this.viewModel != null)
            {
                this.DisplayName = $"{Resources.Title_ColumnInfo}({this.GetHeader(this.viewModel)})";
                this.tableInfo = this.GetTableInfo(this.viewModel);
                this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
            else
            {
                this.tableInfo = null;
            }

            if (this.tableInfo != null)
                this.columns = this.tableInfo.Value.Columns.Select(item => new TemplateColumnInfoItemViewModel(item)).ToArray();
            else
                this.columns = new TemplateColumnInfoItemViewModel[] { };

            this.NotifyOfPropertyChange(nameof(this.Columns));
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        public bool IsLeft
        {
            get; set;
        }

        protected abstract TableInfo? GetTableInfo(TemplateTreeViewItemViewModel item);

        protected abstract string GetHeader(TemplateTreeViewItemViewModel item);

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TemplateTreeViewItemViewModel.IsResolved) || e.PropertyName == string.Empty)
            {
                this.SelectObject(this.viewModel);
            }
        }
    }
}
