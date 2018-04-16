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
using Ntreev.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    class NewChildTableTemplate : TableTemplateBase
    {
        private readonly Table parent;
        private Table table;

        public NewChildTableTemplate(Table parent)
        {
            this.parent = parent;
            this.parent.Attach(this);
            this.IsNew = true;
        }

        public override void OnValidateBeginEdit(Authentication authentication, object target)
        {
            base.OnValidateBeginEdit(authentication, target);

            this.parent.ValidateAccessType(authentication, AccessType.Master);

            if (this.table != null)
                throw new InvalidOperationException(Resources.Exception_Expired);
        }

        public override void OnValidateEndEdit(Authentication authentication, object target)
        {
            base.OnValidateEndEdit(authentication, target);

            this.parent.ValidateAccessType(authentication, AccessType.Master);
        }

        public override void OnValidateCancelEdit(Authentication authentication, object target)
        {
            base.OnValidateCancelEdit(authentication, target);

            this.parent.ValidateAccessType(authentication, AccessType.Master);
        }

        public override Type GetType(string typeName)
        {
            this.Dispatcher.VerifyAccess();
            var typeContext = this.parent.GetService(typeof(TypeContext)) as TypeContext;
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
            get { return this.parent.GetService(typeof(DomainContext)) as DomainContext; }
        }

        public override string ItemPath
        {
            get { return this.parent.Path; }
        }

        public override CremaHost CremaHost
        {
            get { return this.parent.CremaHost; }
        }

        public override CremaDispatcher Dispatcher
        {
            get { return this.parent.Dispatcher; }
        }

        public override DataBase DataBase
        {
            get { return this.parent.DataBase; }
        }

        public override IPermission Permission
        {
            get { return this.parent; }
        }

        protected override void OnBeginEdit(Authentication authentication)
        {
            base.OnBeginEdit(authentication);
        }

        protected override void OnEndEdit(Authentication authentication, CremaTemplate template)
        {
            base.OnEndEdit(authentication, template);
            this.table = this.parent.AddNew(authentication, template);
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
        }

        protected override CremaTemplate CreateSource(Authentication authentication)
        {
            var dataSet = this.parent.ReadAll(authentication);
            var dataTable = dataSet.Tables[this.parent.TableName, this.parent.Category.Path];
            return CremaTemplate.CreateChild(dataTable);
        }
    }
}
