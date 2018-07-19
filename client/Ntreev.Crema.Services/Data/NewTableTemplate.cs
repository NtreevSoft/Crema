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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.DataBaseService;
using Ntreev.Crema.Services.Domains;
using System;

namespace Ntreev.Crema.Services.Data
{
    class NewTableTemplate : TableTemplateBase
    {
        private object parent;
        private Table[] tables;

        public NewTableTemplate(TableCategory category)
        {
            this.parent = category ?? throw new ArgumentNullException(nameof(category));
            this.DomainContext = category.GetService(typeof(DomainContext)) as DomainContext;
            this.ItemPath = category.Path;
            this.CremaHost = category.CremaHost;
            this.Dispatcher = category?.Dispatcher;
            this.DataBase = category.DataBase;
            this.Permission = category;
            this.Service = category.Service;
            this.IsNew = true;
        }

        public NewTableTemplate(Table parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.DomainContext = parent.GetService(typeof(DomainContext)) as DomainContext;
            this.ItemPath = parent.Path;
            this.CremaHost = parent.CremaHost;
            this.Dispatcher = parent?.Dispatcher;
            this.DataBase = parent.DataBase;
            this.Permission = parent;
            this.Service = parent.Service;
            this.IsNew = true;
        }

        public override object Target
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.tables;
            }
        }

        public override DomainContext DomainContext { get; }

        public override string ItemPath { get; }

        public override CremaHost CremaHost { get; }

        public override CremaDispatcher Dispatcher { get; }

        public override DataBase DataBase { get; }

        public override IPermission Permission { get; }

        public IDataBaseService Service { get; }

        protected override void OnBeginEdit(Authentication authentication, DomainMetaData metaData)
        {
            base.OnBeginEdit(authentication, metaData);
        }

        protected override void OnEndEdit(Authentication authentication, TableInfo[] tableInfos)
        {
            base.OnEndEdit(authentication, tableInfos);
            if (this.parent is TableCategory category)
            {
                var tables = category.GetService(typeof(TableCollection)) as TableCollection;
                this.tables = tables.AddNew(authentication, tableInfos);
            }
            else if (this.parent is Table table)
            {
                var tables = table.GetService(typeof(TableCollection)) as TableCollection;
                this.tables = tables.AddNew(authentication, tableInfos);
            }
            this.parent = null;
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
            this.parent = null;
        }

        protected override ResultBase<DomainMetaData> BeginDomain(Authentication authentication)
        {
            return this.Service.BeginNewTable(this.ItemPath);
        }

        protected override ResultBase<TableInfo[]> EndDomain(Authentication authentication, Guid domainID)
        {
            return this.Service.EndTableTemplateEdit(domainID);
        }

        protected override ResultBase CancelDomain(Authentication authentication, Guid domainID)
        {
            return this.Service.CancelTableTemplateEdit(domainID);
        }
    }
}
