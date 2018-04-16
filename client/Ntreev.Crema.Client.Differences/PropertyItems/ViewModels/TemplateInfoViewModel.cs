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
using Ntreev.Crema.Client.Differences.PropertyItems.Views;
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
    abstract class TemplateInfoViewModel : PropertyItemBase
    {
        private TemplateTreeViewItemViewModel viewModel;
        private TableInfo tableInfo;
        private CremaTemplate template;

        public TemplateInfoViewModel()
        {
            this.DisplayName = Resources.Title_TableInfo;
        }

        public TableInfo TableInfo
        {
            get { return this.tableInfo; }
            set
            {
                this.tableInfo = value;
                this.NotifyOfPropertyChange(nameof(this.TableInfo));
            }
        }

        public override bool IsVisible
        {
            get { return this.viewModel != null; }
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
            this.Detach();
            this.viewModel = obj as TemplateTreeViewItemViewModel;
            this.Attach();
        }

        protected abstract CremaTemplate GetTemplate(TemplateTreeViewItemViewModel item);

        protected abstract string GetHeader(TemplateTreeViewItemViewModel item);

        private void Template_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaTemplate.TableInfo) || e.PropertyName == string.Empty)
            {
                this.TableInfo = this.viewModel.Source1.TableInfo;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        private void Attach()
        {
            if (this.viewModel != null)
            {
                this.template = this.GetTemplate(this.viewModel);
                this.template.PropertyChanged += Template_PropertyChanged;
                this.DisplayName = this.GetHeader(this.viewModel);
                this.TableInfo = this.template.TableInfo;
            }

            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        private void Detach()
        {
            if (this.template != null)
            {
                this.template.PropertyChanged -= Template_PropertyChanged;
            }
            this.template = null;
        }
    }
}
