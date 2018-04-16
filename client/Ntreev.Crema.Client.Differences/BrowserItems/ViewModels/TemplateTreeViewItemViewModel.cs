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

using Ntreev.Crema.Client.Differences.Documents.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Client.Differences.BrowserItems.ViewModels
{
    class TemplateTreeViewItemViewModel : DifferenceTreeViewItemViewModel
    {
        private readonly DiffTemplate diffTemplate;
        private string header1;
        private string header2;
        private ICommand viewCommand;
        private bool isActivated;

        [Import]
        private DocumentServiceViewModel documentService = null;

        public TemplateTreeViewItemViewModel(BrowserViewModel browser, DiffTemplate diffTemplate)
            : base(browser)
        {
            this.diffTemplate = diffTemplate;
            this.diffTemplate.PropertyChanged += DiffTemplate_PropertyChanged;
            this.diffTemplate.SourceItem1.PropertyChanged += Template1_PropertyChanged;
            this.diffTemplate.SourceItem2.PropertyChanged += Template2_PropertyChanged;
            this.header1 = diffTemplate.Header1;
            this.header2 = diffTemplate.Header2;
            this.viewCommand = new DelegateCommand(this.View);
            this.isActivated = diffTemplate.DiffState != DiffState.Unchanged;
            this.Target = diffTemplate;

            foreach (var item in this.diffTemplate.DiffTable.Childs)
            {
                this.Items.Add(new TemplateTreeViewItemViewModel(browser, item.Template));
            }
            this.Dispatcher.InvokeAsync(() =>
            {
                if (this.DiffState != DiffState.Unchanged && this.Parent != null)
                {
                    this.Parent.IsExpanded = true;
                }
            });
        }

        public override string ToString()
        {
            return this.DisplayName;
        }

        public void View()
        {
            this.documentService.View(this);
        }

        public override string DisplayName
        {
            get
            {
                if (this.diffTemplate.ItemName1 == diffTemplate.ItemName2)
                    return this.diffTemplate.ItemName1;
                return $"{this.diffTemplate.ItemName1} => {this.diffTemplate.ItemName2}";
            }
        }

        public DiffState DiffState
        {
            get { return this.diffTemplate.DiffState; }
        }

        public bool IsResolved
        {
            get { return this.diffTemplate.IsResolved; }
        }

        public bool IsActivated
        {
            get { return this.isActivated; }
        }

        public DiffTemplate Source
        {
            get { return this.diffTemplate; }
        }

        public CremaTemplate Source1
        {
            get { return this.diffTemplate.SourceItem1; }
        }

        public CremaTemplate Source2
        {
            get { return this.diffTemplate.SourceItem2; }
        }

        public IEnumerable<object> UnresolvedItems
        {
            get { return this.diffTemplate.UnresolvedItems; }
        }

        public string Header1
        {
            get { return this.header1 ?? string.Empty; }
            set
            {
                this.header1 = value;
                this.NotifyOfPropertyChange(() => this.Header1);
            }
        }

        public string Header2
        {
            get { return this.header2; }
            set
            {
                this.header2 = value;
                this.NotifyOfPropertyChange(() => this.Header2);
            }
        }

        public ICommand ViewCommand
        {
            get { return this.viewCommand; }
        }

        private void DiffTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.diffTemplate.DiffState) || e.PropertyName == string.Empty)
            {
                this.NotifyOfPropertyChange(nameof(this.DiffState));
            }

            if (e.PropertyName == nameof(this.diffTemplate.IsResolved))
            {
                this.NotifyOfPropertyChange(nameof(this.IsResolved));
            }

            //if (e.PropertyName == nameof(this.diffTemplate.Name) || e.PropertyName == string.Empty)
            //{
            //    this.NotifyOfPropertyChange(nameof(this.DisplayName));
            //}
        }

        private void Template1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaTemplate.TableName))
            {
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        private void Template2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaTemplate.TableName))
            {
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }
    }
}
