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
using System;

namespace Ntreev.Crema.Services.Data
{
    class TypeTemplate : TypeTemplateBase
    {
        private readonly Type type;

        public TypeTemplate(Type type)
        {
            this.type = type;
        }

        public override DomainContext DomainContext => this.type.GetService(typeof(DomainContext)) as DomainContext;

        public override string ItemPath => this.type.Path;

        public override CremaDispatcher Dispatcher => this.type.Dispatcher;

        public override CremaHost CremaHost => this.type.CremaHost;

        public override IType Type => this.type;

        public override DataBase DataBase => this.type.DataBase;

        public override IPermission Permission => this.type;

        public override void OnValidateBeginEdit(Authentication authentication, object target)
        {
            base.OnValidateBeginEdit(authentication, target);

            this.type.ValidateNotBeingEdited();
            this.type.ValidateAccessType(authentication, AccessType.Master);
            this.type.ValidateUsingTables(authentication);
        }

        public override void OnValidateEndEdit(Authentication authentication, object target)
        {
            base.OnValidateEndEdit(authentication, target);

            if (this.TypeSource == null)
                throw new InvalidOperationException(Resources.Exception_CannotEndEdit);
            this.type.ValidateUsingTables(authentication);
        }

        public override void OnValidateCancelEdit(Authentication authentication, object target)
        {
            base.OnValidateCancelEdit(authentication, target);
            this.type.ValidateAccessType(authentication, AccessType.Master);
        }

        protected override void OnBeginEdit(Authentication authentication)
        {
            this.Container.InvokeTypeBeginTemplateEdit(authentication, this.type);
            base.OnBeginEdit(authentication);
            this.type.IsBeingEdited = true;
            this.Container.InvokeTypesStateChangedEvent(authentication, new Type[] { this.type });
        }

        protected override void OnEndEdit(Authentication authentication)
        {
            this.Container.InvokeTypeEndTemplateEdit(authentication, this.type, this.TypeSource.DataSet);
            base.OnEndEdit(authentication);
            this.type.UpdateTypeInfo(this.TypeSource.TypeInfo);
            this.type.IsBeingEdited = false;
            this.Container.InvokeTypesStateChangedEvent(authentication, new Type[] { this.type });
            this.Container.InvokeTypesChangedEvent(authentication, new Type[] { this.type }, this.TypeSource.DataSet);
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
            this.type.IsBeingEdited = false;
            this.Container.InvokeTypesStateChangedEvent(authentication, new Type[] { this.type });
        }

        protected override void OnRestore(Domain domain)
        {
            this.type.IsBeingEdited = true;
            base.OnRestore(domain);
        }

        protected override CremaDataType CreateSource(Authentication authentication)
        {
            var dataSet = this.type.ReadAllData(authentication);
            return dataSet.Types[this.type.Name, this.type.Category.Path];
        }

        private TypeCollection Container
        {
            get { return this.type.Container; }
        }
    }
}
