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
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Library;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Client.Framework
{
    public class TableContentListItemBase : DescriptorListItemBase<TableContentDescriptor>, ITableContentDescriptor
    {
        //private IDomain domain;

        public TableContentListItemBase(Authentication authentication, ITableContent content, object owner)
            : base(authentication, new TableContentDescriptor(authentication, content, DescriptorTypes.IsSubscriptable, owner), owner)
        {

        }

        //public TableContentListItemBase(Authentication authentication, ITableContentDescriptor descriptor, object owner)
        //    : base(authentication, new TableContentDescriptor(authentication, descriptor, DescriptorTypes.IsSubscriptable, owner), owner)
        //{

        //}

        public TableContentListItemBase(Authentication authentication, TableContentDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {

        }

        public async Task BeginEditAsync()
        {
            await TableContentDescriptorUtility.BeginEditAsync(this.authentication, this.descriptor);
        }

        public string Name => this.descriptor.Name;

        public string TableName => this.descriptor.TableName;

        public string Path => this.descriptor.Path;

        public override string DisplayName => this.descriptor.Name;

        public bool IsModified => this.descriptor.IsModified;

        public DomainAccessType AccessType => this.descriptor.AccessType;

        public IDomain TargetDomain => this.descriptor.TargetDomain;

        #region ITableContentDescriptor

        ITableContent ITableContentDescriptor.Target => this.descriptor.Target as ITableContent;

        #endregion
    }
}
