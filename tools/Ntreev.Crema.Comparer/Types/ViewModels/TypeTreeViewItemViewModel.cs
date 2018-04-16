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

namespace Ntreev.Crema.Comparer.Types.ViewModels
{
    public class TypeTreeViewItemViewModel : TreeViewItemViewModel
    {
        private readonly DiffDataType diffType;
        private string header1;
        private string header2;
        private ICommand viewCommand;

        [Import]
        private TypeDocumentServiceViewModel documentService = null;

        public TypeTreeViewItemViewModel(DiffDataType diffType)
        {
            this.diffType = diffType;
            this.diffType.PropertyChanged += DiffType_PropertyChanged;
            this.diffType.SourceItem1.PropertyChanged += DataType1_PropertyChanged;
            this.diffType.SourceItem2.PropertyChanged += DataType2_PropertyChanged;
            this.viewCommand = new DelegateCommand(this.View);
            this.Target = diffType;
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
                if (this.diffType.SourceItem1.TypeName == diffType.SourceItem2.TypeName)
                    return this.diffType.SourceItem1.TypeName;
                return $"{this.diffType.SourceItem1.TypeName} => {this.diffType.SourceItem2.TypeName}";
            }
        }

        public DiffState DiffState
        {
            get { return this.diffType.DiffState; }
        }

        public bool IsResolved
        {
            get { return this.diffType.IsResolved; }
        }

        public DiffDataType Source
        {
            get { return this.diffType; }
        }

        public CremaDataType Source1
        {
            get { return this.diffType.SourceItem1; }
        }

        public CremaDataType Source2
        {
            get { return this.diffType.SourceItem2; }
        }

        public IEnumerable<object> UnresolvedItems
        {
            get { return new object[] { }; }
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

        public bool IsFlag
        {
            get { return this.Source2.IsFlag; }
        }

        private void DiffType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.diffType.DiffState) || e.PropertyName == string.Empty)
            {
                this.NotifyOfPropertyChange(nameof(this.DiffState));
            }

            if (e.PropertyName == nameof(this.diffType.IsResolved))
            {
                this.NotifyOfPropertyChange(nameof(this.IsResolved));
            }

            //if (e.PropertyName == nameof(this.diffType.Name) || e.PropertyName == string.Empty)
            //{
            //    this.NotifyOfPropertyChange(nameof(this.DisplayName));
            //}
        }

        private void DataType1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaDataType.TypeName))
            {
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        private void DataType2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaDataType.TypeName))
            {
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }
    }
}
