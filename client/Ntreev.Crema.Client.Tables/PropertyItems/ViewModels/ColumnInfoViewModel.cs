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

using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Data;
using Ntreev.Crema.Client.Tables.Properties;

namespace Ntreev.Crema.Client.Tables.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [RequiredAuthority(Authority.Guest)]
    [Dependency(typeof(TableInfoViewModel))]
    [ParentType(typeof(PropertyService))]
    class ColumnInfoViewModel : PropertyItemBase
    {
        private TableInfo tableInfo;
        private ITableDescriptor descriptor;
        private ColumnInfoItemViewModel[] columns;

        public ColumnInfoViewModel()
        {
            this.DisplayName = Resources.Title_TableColumnInfo;
        }

        public override bool CanSupport(object obj)
        {
            return obj is ITableDescriptor;
        }

        public override void SelectObject(object obj)
        {
            this.Detach();
            this.descriptor = obj as ITableDescriptor;
            this.Attach();
        }

        public override object SelectedObject
        {
            get { return this.descriptor; }
        }

        public TableInfo TableInfo
        {
            get { return this.tableInfo; }
            set
            {
                this.tableInfo = value;
                this.Columns = value.Columns.Select(item => new ColumnInfoItemViewModel(item)).ToArray();
                this.NotifyOfPropertyChange(nameof(TableInfo));
            }
        }

        public ColumnInfoItemViewModel[] Columns
        {
            get { return this.columns; }
            set
            {
                this.columns = value;
                this.NotifyOfPropertyChange(nameof(this.Columns));
            }
        }

        public override bool IsVisible
        {
            get { return this.descriptor != null; }
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ITableDescriptor.TableInfo) || e.PropertyName == string.Empty)
            {
                this.TableInfo = this.descriptor.TableInfo;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        private void Attach()
        {
            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged += Descriptor_PropertyChanged;
                }
                this.TableInfo = this.descriptor.TableInfo;
            }

            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        private void Detach()
        {
            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged -= Descriptor_PropertyChanged;
                }
            }
        }
    }
}
