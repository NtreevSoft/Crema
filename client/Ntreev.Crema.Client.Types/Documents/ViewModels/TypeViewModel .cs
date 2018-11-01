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
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.ModernUI.Framework;
using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Types.Documents.ViewModels
{
    [Export(typeof(ITypeDocument))]
    class TypeViewModel : DocumentBase, ITypeDocument
    {
        private readonly Authentication authentication;
        private readonly IType type;
        private CremaDataType dataType;
        private object selectedItem;
        private string selectedColumn;

        public TypeViewModel(Authentication authentication, IType type)
        {
            this.authentication = authentication;
            this.type = type;
            this.Initialize();
        }

        public IType Target
        {
            get { return this.type; }
        }

        public CremaDataType Source
        {
            get { return this.dataType; }
            private set
            {
                this.dataType = value;
                this.IsModified = false;
                this.NotifyOfPropertyChange(nameof(this.Source));
            }
        }

        public object SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
            }
        }

        public int SelectedItemIndex
        {
            set
            {
                if (value >= 0 && value < this.dataType.View.Count)
                {
                    var item = this.dataType.View[value];
                    this.SelectedItem = item;
                }
                else
                {
                    this.SelectedItem = null;
                }
            }
        }

        public string SelectedColumn
        {
            get { return this.selectedColumn; }
            set
            {
                this.selectedColumn = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedColumn));
            }
        }

        public override string DisplayName
        {
            get
            {
                if (this.Source == null)
                    return string.Empty;
                return this.Source.TypeName;
            }
        }

        protected override async void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);

            if (this.type.Dispatcher == null) return;

            await this.type.Dispatcher.InvokeAsync(() =>
            {
                this.type.TypeInfoChanged -= Type_TypeInfoChanged;
            });
        }

        private async void Initialize()
        {
            this.BeginProgress();
            var itemsSource = await this.type.Dispatcher.InvokeAsync(() =>
             {
                 var dataSet = this.type.GetDataSet(this.authentication, -1);
                 this.type.TypeInfoChanged += Type_TypeInfoChanged;
                 return dataSet.Types.FirstOrDefault();
             });
            this.Source = itemsSource;
            this.EndProgress();
            this.Refresh();
        }

        private async void Type_TypeInfoChanged(object sender, EventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.BeginProgress();
            });

            var dataSet = await this.type.Dispatcher.InvokeAsync(() => this.type.GetDataSet(this.authentication, -1));

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.Source = dataSet.Types.FirstOrDefault();
                this.Refresh();
                this.EndProgress();
            }, DispatcherPriority.Background);
        }
    }
}
