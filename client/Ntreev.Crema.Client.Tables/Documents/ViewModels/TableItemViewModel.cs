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
using Ntreev.Crema.Services;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Tables.Documents.ViewModels
{
    class TableItemViewModel : TableListItemBase, ITableDocumentItem, ITableContentDescriptor
    {
        private readonly TableContentDescriptor contentDescriptor;
        private CremaDataTable dataTable;
        private IDomain domain;
        private object selectedItem;
        private int selectedIndex;
        private string selectedColumn;

        public TableItemViewModel(Authentication authentication, TableDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.contentDescriptor = descriptor.ContentDescriptor;
            //if (this.contentDescriptor.TargetDomain is IDomain domain && domain.Source is CremaDataSet dataSet)
            //{
            //    this.dataTable = dataSet.Tables[this.Name];
            //}
        }

        //public TableItemViewModel(Authentication authentication, TableDescriptor descriptor, CremaDataSet dataSet, object owner)
        //    : base(authentication, descriptor, owner)
        //{
        //    this.contentDescriptor = descriptor.ContentDescriptor;
        //    this.dataTable = dataSet.Tables[this.Name];
        //    this.isReadOnly = true;
        //}

        public override string DisplayName => this.descriptor.TableName;

        public bool IsReadOnly
        {
            get { return this.domain == null; }
        }

        public CremaDataTable Source
        {
            get { return this.dataTable; }
            set
            {
                this.dataTable = value;
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
            get { return this.selectedIndex; }
            set
            {
                this.selectedIndex = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedItemIndex));
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

        public IDomain Domain
        {
            get { return this.domain; }
            set
            {
                this.domain = value;
                this.NotifyOfPropertyChange(nameof(this.Domain));
                this.NotifyOfPropertyChange(nameof(this.IsReadOnly));
            }
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);

            this.dataTable = null;
            this.NotifyOfPropertyChange(nameof(this.Source));
            this.NotifyOfPropertyChange(nameof(this.Domain));
        }

        #region ITableDocumentItem

        ITable ITableDocumentItem.Target => this.Target as ITable;

        #endregion

        #region ITableContentDescriptor

        string ITableContentDescriptor.DisplayName => this.contentDescriptor.DisplayName;

        bool ITableContentDescriptor.IsModified => this.contentDescriptor.IsModified;

        DomainAccessType ITableContentDescriptor.AccessType => this.contentDescriptor.AccessType;

        IDomain ITableContentDescriptor.TargetDomain => this.contentDescriptor.TargetDomain;

        ITableContent ITableContentDescriptor.Target => this.contentDescriptor.Target as ITableContent;

        #endregion
    }
}
