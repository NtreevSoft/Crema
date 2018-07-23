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
using System;

namespace Ntreev.Crema.Services.Data
{
    class NewTypeTemplate : TypeTemplateBase
    {
        private readonly TypeCategory category;
        private Type type;

        public NewTypeTemplate(TypeCategory category)
        {
            this.category = category;
            this.IsNew = true;
        }

        public override IType Type => this.type;

        public override DomainContext DomainContext => this.category.GetService(typeof(DomainContext)) as DomainContext;

        public override CremaDispatcher Dispatcher => this.category?.Dispatcher;

        public override CremaHost CremaHost => this.category.CremaHost;

        public override DataBase DataBase => this.category.DataBase;

        public override IPermission Permission => this.category;

        public TypeCollection Types => this.category.Context.Types;

        protected override void OnBeginEdit(Authentication authentication, DomainMetaData metaData)
        {
            base.OnBeginEdit(authentication, metaData);
        }

        protected override void OnEndEdit(Authentication authentication, TypeInfo typeInfo)
        {
            base.OnEndEdit(authentication, typeInfo);
            this.type = this.Types.AddNew(authentication, typeInfo);
        }

        protected override void OnCancelEdit(Authentication authentication)
        {
            base.OnCancelEdit(authentication);
        }

        protected override ResultBase<DomainMetaData> BeginDomain(Authentication authentication)
        {
            return this.category.Service.BeginNewType(this.category.Path);
        }

        protected override ResultBase<TypeInfo> EndDomain(Authentication authentication, Guid domainID)
        {
            return this.category.Service.EndTypeTemplateEdit(domainID);
        }

        protected override ResultBase CancelDomain(Authentication authentication, Guid domainID)
        {
            return this.category.Service.CancelTypeTemplateEdit(domainID);
        }
    }
}
