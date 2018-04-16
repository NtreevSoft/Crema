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

using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Extensions;
using Ntreev.Library.Serialization;

namespace Ntreev.Crema.Services.Data
{
    class NewTableTemplate : TableTemplateBase
    {
        private readonly TableCategory category;
        private Table table;

        public NewTableTemplate(TableCategory category)
        {
            this.category = category;
            this.IsNew = true;
        }

        public override Type GetType(string typeName)
        {
            this.Dispatcher.VerifyAccess();
            var typeContext = this.category.GetService(typeof(TypeContext)) as TypeContext;
            return typeContext[typeName] as Type;
        }

        public override ITable Table
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.table;
            }
        }

        public override DomainContext DomainContext
        {
            get { return this.category.GetService(typeof(DomainContext)) as DomainContext; }
        }

        public override string ItemPath
        {
            get { return this.category.Path; }
        }

        public override CremaHost CremaHost
        {
            get { return this.category.CremaHost; }
        }

        public override CremaDispatcher Dispatcher
        {
            get { return this.category.Dispatcher; }
        }

        public override DataBase DataBase
        {
            get { return this.category.DataBase; }
        }

        public override IPermission Permission
        {
            get { return this.category; }
        }

        protected override void OnBeginEdit(Authentication authentication, DomainMetaData metaData)
        {
            base.OnBeginEdit(authentication, metaData);
        }

        protected override void OnEndEdit(Authentication authentication, TableInfo tableInfo)
        {
            base.OnEndEdit(authentication, tableInfo);
            this.table = this.category.Context.Tables.AddNew(authentication, tableInfo);
            this.table.Container.InvokeTablesCreatedEvent(authentication, new Table[] { this.table, });
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
        }

        protected override ResultBase<DomainMetaData> BeginDomain(Authentication authentication)
        {
            return this.category.Service.BeginNewTable(this.category.Path);
        }

        protected override ResultBase<TableInfo> EndDomain(Authentication authentication, Guid domainID)
        {
            return this.category.Service.EndTableTemplateEdit(domainID);
        }

        protected override ResultBase CancelDomain(Authentication authentication, Guid domainID)
        {
            return this.category.Service.CancelTableTemplateEdit(domainID);
        }
    }
}
