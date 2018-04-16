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
using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library.Linq;
using System;
using System.ComponentModel;
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

        public override DomainContext DomainContext
        {
            get { return this.table.GetService(typeof(DomainContext)) as DomainContext; }
        }

        public override string ItemPath
        {
            get { return this.table.Path; }
        }

        public override CremaHost CremaHost
        {
            get { return this.table.CremaHost; }
        }

        public override CremaDispatcher Dispatcher
        {
            get { return this.table.Dispatcher; }
        }

        public override DataBase DataBase
        {
            get { return this.table.DataBase; }
        }

        public override IPermission Permission
        {
            get { return this.table; }
        }

        protected override void OnBeginEdit(Authentication authentication)
        {
            base.OnBeginEdit(authentication);
            this.table.SetTableState(TableState.IsBeingSetup);
            this.Container.InvokeTablesStateChangedEvent(authentication, new Table[] { this.table, });
        }

        protected override void OnEndEdit(Authentication authentication, CremaTemplate template)
        {
            this.Container.InvokeTableEndTemplateEdit(authentication, this.table, this.TemplateSource);
            base.OnEndEdit(authentication, template);
            this.table.UpdateTemplate(template.TableInfo);
            this.table.UpdateTags(template.Tags);
            this.table.UpdateComment(template.Comment);
            this.table.SetTableState(TableState.None);

            var items = EnumerableUtility.One(this.table).ToArray();
            this.Container.InvokeTablesStateChangedEvent(authentication, items);
            this.Container.InvokeTablesTemplateChangedEvent(authentication, items, this.TemplateSource.TargetTable.DataSet);
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
            this.table.SetTableState(TableState.None);
            this.Container.InvokeTablesStateChangedEvent(authentication, new Table[] { this.table });
        }

        protected override void OnRestore(Domain domain)
        {
            this.table.SetTableState(TableState.IsBeingSetup);
            base.OnRestore(domain);
        }

        protected override CremaTemplate CreateSource(Authentication authentication)
        {
            var dataSet = this.table.ReadAll(authentication);
            return new CremaTemplate(dataSet.Tables[this.table.Name, this.table.Category.Path]);
        }

        private TableCollection Container
        {
            get { return this.table.Container; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateBeginEdit(Authentication authentication, object target)
        {
            base.OnValidateBeginEdit(authentication, target);

            if (target == this && this.table.TemplatedParent != null)
                throw new InvalidOperationException(Resources.Exception_InheritedTableCannotEditTemplate);

            this.table.ValidateAccessType(authentication, AccessType.Master);
            this.table.ValidateNotBeingEdited();
            this.table.ValidateHasNotBeingEditedType();

            var templates = this.table.Childs.Select(item => item.Template);
            foreach (var item in templates)
            {
                item.OnValidateBeginEdit(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateEndEdit(Authentication authentication, object target)
        {
            base.OnValidateEndEdit(authentication, target);

            if (target == this)
            {
                if (this.IsBeingEdited == false)
                    throw new CremaException("템플릿이 편집중이 아닙니다.");
                this.TemplateSource.Validate();
            }

            this.table.ValidateAccessType(authentication, AccessType.Master);

            var templates = this.table.Childs.Select(item => item.Template);
            foreach (var item in templates)
            {
                item.OnValidateEndEdit(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateCancelEdit(Authentication authentication, object target)
        {
            base.OnValidateCancelEdit(authentication, target);

            if (target == this)
            {
                if (this.IsBeingEdited == false)
                    throw new CremaException("템플릿이 편집중이 아닙니다.");
            }

            this.table.ValidateAccessType(authentication, AccessType.Master);

            var templates = this.table.Childs.Select(item => item.Template);
            foreach (var item in templates)
            {
                item.OnValidateCancelEdit(authentication, target);
            }
        }
    }
}
