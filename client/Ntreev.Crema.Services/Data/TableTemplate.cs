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
using Ntreev.Library.Linq;
using System;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TableTemplate : TableTemplateBase
    {
        private readonly Table table;

        public TableTemplate(Table table)
        {
            this.table = table;
        }

        public override Type GetType(string typeName)
        {
            var typeContext = this.table.GetService(typeof(TypeContext)) as TypeContext;
            return typeContext[typeName] as Type;
        }

        public override ITable Table
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.table;
            }
        }

        public override DomainContext DomainContext => this.table.GetService(typeof(DomainContext)) as DomainContext;

        public override string ItemPath => this.table.Path;

        public override CremaHost CremaHost => this.table.CremaHost;

        public override CremaDispatcher Dispatcher => this.table.Dispatcher;

        public override DataBase DataBase => this.table.DataBase;

        public override IPermission Permission => this.table;

        protected override void OnBeginEdit(Authentication authentication, DomainMetaData metaData)
        {
            base.OnBeginEdit(authentication, metaData);
            this.table.SetTableState(TableState.IsBeingSetup | TableState.IsMember);
            this.Container.InvokeTablesStateChangedEvent(authentication, new Table[] { this.table, });
        }

        protected override void OnEndEdit(Authentication authentication, TableInfo tableInfo)
        {
            this.Container.InvokeTableEndTemplateEdit(authentication, this.table);
            base.OnEndEdit(authentication, tableInfo);
            this.table.UpdateTemplate(tableInfo);
            this.table.UpdateTags(tableInfo.Tags);
            this.table.UpdateComment(tableInfo.Comment);
            this.table.SetTableState(TableState.None);

            var items = EnumerableUtility.One(this.table).ToArray();
            this.Container.InvokeTablesStateChangedEvent(authentication, items);
            this.Container.InvokeTablesTemplateChangedEvent(authentication, items);
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
            this.table.SetTableState(TableState.None);
            this.Container.InvokeTablesStateChangedEvent(authentication, new Table[] { this.table });
        }

        protected override ResultBase<DomainMetaData> BeginDomain(Authentication authentication)
        {
            return this.Service.BeginTableTemplateEdit(this.table.Name);
        }

        protected override ResultBase<TableInfo> EndDomain(Authentication authentication, Guid domainID)
        {
            return this.Service.EndTableTemplateEdit(domainID);
        }

        protected override ResultBase CancelDomain(Authentication authentication, Guid domainID)
        {
            return this.Service.CancelTableTemplateEdit(domainID);
        }

        private TableCollection Container => this.table.Container;

        private IDataBaseService Service => this.table.Service;
    }
}
