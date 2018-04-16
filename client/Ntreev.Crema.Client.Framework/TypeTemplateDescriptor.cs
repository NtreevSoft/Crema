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

using System;
using System.Linq;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ServiceModel;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections;
using Ntreev.Library;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class TypeTemplateDescriptor : DescriptorBase, ITypeTemplateDescriptor
    {
        private ITypeTemplate template;
        private readonly object owner;

        private IDomain domain;
        private TypeInfo typeInfo = TypeInfo.Default;
        private bool isModified;

        public TypeTemplateDescriptor(Authentication authentication, ITypeTemplate type)
            : this(authentication, type, DescriptorTypes.All)
        {

        }

        public TypeTemplateDescriptor(Authentication authentication, ITypeTemplate template, DescriptorTypes descriptorTypes)
            : this(authentication, template, descriptorTypes, null)
        {

        }

        public TypeTemplateDescriptor(Authentication authentication, ITypeTemplate template, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, template, descriptorTypes)
        {
            this.template = template;
            this.owner = owner ?? this;
            this.template.Dispatcher.VerifyAccess();
            this.domain = this.template.Domain;

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.template.EditBegun += Template_EditBegun;
                this.template.EditEnded += Template_EditEnded;
                this.template.EditCanceled += Template_EditCanceled;
                this.template.Changed += Template_Changed;
                this.template.Type.TypeInfoChanged += Type_TypeInfoChanged;
            }
        }

        [DescriptorProperty]
        public string Name => this.typeInfo.Name;

        [DescriptorProperty]
        public string Path => this.typeInfo.CategoryPath + this.typeInfo.Name;

        [DescriptorProperty]
        public string DisplayName => this.typeInfo.Name;

        [DescriptorProperty]
        public bool IsModified => this.isModified;

        [DescriptorProperty]
        public IDomain TargetDomain => this.domain;

        public event EventHandler EditBegun;

        public event EventHandler EditEnded;

        public event EventHandler EditCanceled;

        protected virtual void OnEditBegun(EventArgs e)
        {
            this.EditBegun?.Invoke(this, e);
        }

        protected virtual void OnEditEnded(EventArgs e)
        {
            this.EditEnded?.Invoke(this, e);
        }

        protected virtual void OnEditCanceled(EventArgs e)
        {
            this.EditCanceled?.Invoke(this, e);
        }

        private void Template_EditBegun(object sender, EventArgs e)
        {
            this.domain = this.template.Domain;
            this.Dispatcher.InvokeAsync(async () =>
            {
                await this.RefreshAsync();
                this.OnEditBegun(e);
            });
        }

        private void Template_EditEnded(object sender, EventArgs e)
        {
            this.domain = null;
            this.Dispatcher.InvokeAsync(async () =>
            {
                await this.RefreshAsync();
                this.OnEditEnded(e);
            });
        }

        private void Template_EditCanceled(object sender, EventArgs e)
        {
            this.domain = null;
            this.Dispatcher.InvokeAsync(async () =>
            {
                await this.RefreshAsync();
                this.OnEditCanceled(e);
            });
        }

        private async void Template_Changed(object sender, EventArgs e)
        {
            this.isModified = this.template.IsModified;
            await this.RefreshAsync();
        }

        private async void Type_TypeInfoChanged(object sender, EventArgs e)
        {
            this.typeInfo = this.template.Type.TypeInfo;
            await this.RefreshAsync();
        }

        #region ITypeTemplateDescriptor

        ITypeTemplate ITypeTemplateDescriptor.Target => this.template as ITypeTemplate;

        #endregion
    }
}
