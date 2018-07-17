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
    public class TableContentDescriptor : DescriptorBase, ITableContentDescriptor
    {
        private ITableContent content;
        private readonly object owner;

        private IDomain domain;
        private TableInfo tableInfo = TableInfo.Default;
        private DomainAccessType accessType;
        private bool isModified;

        public TableContentDescriptor(Authentication authentication, ITableContent content, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, content, descriptorTypes)
        {
            this.content = content;
            this.owner = owner ?? this;
            this.content.Dispatcher.VerifyAccess();
            this.domain = this.content.Domain;

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.content.EditBegun += Content_EditBegun;
                this.content.EditEnded += Content_EditEnded;
                this.content.EditCanceled += Content_EditCanceled;
                this.content.Changed += Content_Changed;
                this.content.Table.TableInfoChanged += Table_TableInfoChanged;
            }
        }

        [DescriptorProperty]
        public string Name => this.tableInfo.Name;

        [DescriptorProperty]
        public string TableName => this.tableInfo.TableName;

        [DescriptorProperty]
        public string Path => this.tableInfo.CategoryPath + this.tableInfo.Name;

        [DescriptorProperty]
        public string DisplayName => this.tableInfo.Name;

        [DescriptorProperty]
        public bool IsModified => this.isModified;

        [DescriptorProperty]
        public DomainAccessType AccessType => this.accessType;

        [DescriptorProperty]
        public IDomain TargetDomain => this.domain;

        public event EventHandler EditBegun;

        public event EventHandler EditEnded;

        public event EventHandler EditCanceled;

        public event EventHandler Kicked;

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

        protected virtual void OnKicked(EventArgs e)
        {
            this.Kicked?.Invoke(this, e);
        }

        private async void Content_EditBegun(object sender, EventArgs e)
        {
            this.domain = this.content.Domain;
            await this.domain.Dispatcher.InvokeAsync(() => this.domain.UserRemoved += Domain_UserRemoved);
            await this.Dispatcher.InvokeAsync(async () =>
            {
                await this.RefreshAsync();
                this.OnEditBegun(e);
            });
        }

        private void Content_EditEnded(object sender, EventArgs e)
        {
            this.domain = null;
            this.Dispatcher.InvokeAsync(async () =>
            {
                await this.RefreshAsync();
                this.OnEditEnded(e);
            });
        }

        private void Content_EditCanceled(object sender, EventArgs e)
        {
            this.domain = null;
            this.Dispatcher.InvokeAsync(async () =>
            {
                await this.RefreshAsync();
                this.OnEditCanceled(e);
            });
        }

        private async void Content_Changed(object sender, EventArgs e)
        {
            this.isModified = this.content.IsModified;
            await this.RefreshAsync();
        }

        private void Domain_UserRemoved(object sender, DomainUserRemovedEventArgs e)
        {
            var domainUserID = e.DomainUserInfo.UserID;
            var removeInfo = e.RemoveInfo;
            var userID = e.UserID;
            if (domainUserID == this.authentication.ID && removeInfo.Reason == RemoveReason.Kick)
            {
                this.accessType = DomainAccessType.None;
                this.Dispatcher.InvokeAsync(async () =>
                {
                    await this.RefreshAsync();
                    this.OnKicked(e);
                });
            }
        }

        private async void Table_TableInfoChanged(object sender, EventArgs e)
        {
            this.tableInfo = this.content.Table.TableInfo;
            await this.RefreshAsync();
        }

        #region ITableContentDescriptor

        ITableContent ITableContentDescriptor.Target => this.content as ITableContent;

        #endregion
    }
}
