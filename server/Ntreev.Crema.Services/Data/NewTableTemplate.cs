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

namespace Ntreev.Crema.Services.Data
{
    class NewTableTemplate : TableTemplateBase
    {
        private TableCategory category;
        private Table table;

        public NewTableTemplate(TableCategory category)
        {
            this.category = category ?? throw new ArgumentNullException(nameof(category));
            this.category.Attach(this);
            this.IsNew = true;
        }

        public override void OnValidateBeginEdit(Authentication authentication, object target)
        {
            base.OnValidateBeginEdit(authentication, target);
            if (this.category == null)
                throw new InvalidOperationException(Resources.Exception_Expired);
            if (this.Domain != null)
                throw new CremaException("이미 편집중입니다.");
            this.category.ValidateAccessType(authentication, AccessType.Master);
        }

        public override void OnValidateEndEdit(Authentication authentication, object target)
        {
            base.OnValidateEndEdit(authentication, target);
            this.category.ValidateAccessType(authentication, AccessType.Master);
            this.TemplateSource.Validate();
        }

        public override void OnValidateCancelEdit(Authentication authentication, object target)
        {
            base.OnValidateCancelEdit(authentication, target);
            this.category.ValidateAccessType(authentication, AccessType.Master);
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
                this.Dispatcher?.VerifyAccess();
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
            get
            {
                if (this.category == null)
                    return null;
                return this.category.Dispatcher;
            }
        }

        public override DataBase DataBase
        {
            get { return this.category.DataBase; }
        }

        public override IPermission Permission
        {
            get { return this.category; }
        }

        protected override void OnBeginEdit(Authentication authentication)
        {
            base.OnBeginEdit(authentication);
        }

        protected override void OnEndEdit(Authentication authentication, CremaTemplate template)
        {
            base.OnEndEdit(authentication, template);
            this.table = this.category.Context.Tables.AddNew(authentication, template);
            this.category = null;
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
            this.category = null;
        }

        protected override CremaTemplate CreateSource(Authentication authentication)
        {
            var typeContext = this.category.GetService(typeof(TypeContext)) as TypeContext;
            var dataSet = typeContext.Root.ReadData(authentication, true);
            var newName = NameUtility.GenerateNewName("Table", this.category.Context.Tables.Select((Table item) => item.Name));
            var templateSource = CremaTemplate.Create(dataSet, newName, this.category.Path);
            return templateSource;
        }
    }
}
