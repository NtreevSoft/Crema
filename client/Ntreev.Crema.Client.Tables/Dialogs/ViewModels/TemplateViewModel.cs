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

using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Library;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Client.Tables.Dialogs.ViewModels
{
    public abstract class TemplateViewModel : ModalDialogAppBase
    {
        private readonly Authentication authentication;
        private ITableTemplate template;
        private IDomain domain;
        private bool isNew;
        private bool isReadOnly;
        private bool isModified;
        private bool isValid;
        private string tableName;
        private string comment;
        private TagInfo tags;
        private string[] selectableTypes;
        private int count;
        private object source;
        private bool ignoreCaseSensitive;

        [Import]
        private IFlashService flashService = null;

        protected TemplateViewModel(Authentication authentication, ITableTemplate template)
            : this(authentication, template, false)
        {

        }

        protected TemplateViewModel(Authentication authentication, ITableTemplate template, bool isNew)
        {
            this.authentication = authentication;
            this.isNew = isNew;
            this.template = template;
            this.template.EditEnded += Template_EditEnded;
            this.template.EditCanceled += Template_EditCanceled;
            this.template.Changed += Template_Changed;
            this.DisplayName = Resources.Title_TableTemplateEditing;
        }

        public async void Change()
        {
            try
            {
                this.BeginProgress(this.IsNew ? Resources.Message_Creating : Resources.Message_Changing);
                await this.template.Dispatcher.InvokeAsync(() =>
                {
                    this.template.EndEdit(this.authentication);
                    this.template.EditEnded -= Template_EditEnded;
                    this.template.EditCanceled -= Template_EditCanceled;
                    this.template.Changed -= Template_Changed;
                });
                this.domain = null;
                this.template = null;
                this.isModified = false;
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
            set
            {
                this.isReadOnly = value;
                this.NotifyOfPropertyChange(nameof(this.IsReadOnly));
            }
        }

        public bool IsNew
        {
            get { return this.isNew; }
        }

        public ITableTemplate Template
        {
            get { return this.template; }
        }

        public object Source
        {
            get { return this.source; }
        }

        public IEnumerable SelectableTypes
        {
            get
            {
                if (this.selectableTypes != null)
                {
                    foreach (var item in this.selectableTypes)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IDomain Domain
        {
            get { return this.domain; }
        }

        public string TableName
        {
            get { return this.tableName ?? string.Empty; }
            set
            {
                this.template.Dispatcher.Invoke(() =>
                {
                    this.template.SetTableName(this.authentication, value);
                    this.tableName = value;
                });
                this.NotifyOfPropertyChange(nameof(this.TableName));
                this.Verify(this.VerifyAction);
            }
        }

        public string Comment
        {
            get { return this.comment ?? string.Empty; }
            set
            {
                this.template.Dispatcher.Invoke(() =>
                {
                    this.template.SetComment(this.authentication, value);
                    this.comment = value;
                });
                this.NotifyOfPropertyChange(nameof(this.Comment));
            }
        }

        public bool CanChange
        {
            get
            {
                if (this.IsProgressing == true || this.isReadOnly == true)
                    return false;
                if (this.template == null)
                    return false;
                if (this.count == 0)
                    return false;
                if (this.IsModified == false)
                    return false;
                return this.isValid;
            }
        }

        public bool IsModified
        {
            get { return this.isModified; }
            set
            {
                this.isModified = value;
                this.NotifyOfPropertyChange(nameof(this.IsModified));
                this.NotifyOfPropertyChange(nameof(this.CanChange));
            }
        }

        public TagInfo Tags
        {
            get { return this.tags; }
            set
            {
                this.template.Dispatcher.Invoke(() =>
                {
                    this.template.SetTags(this.authentication, value);
                    this.tags = value;
                });
                this.NotifyOfPropertyChange(nameof(this.Tags));
            }
        }

        public bool IgnoreCaseSensitive
        {
            get => this.ignoreCaseSensitive;
            set
            {
                var originValue = this.ignoreCaseSensitive;
                try
                {
                    this.template.Dispatcher.Invoke(() =>
                    {
                        this.template.SetIgnoreCaseSensitive(this.authentication, value);
                    });
                    this.ignoreCaseSensitive = value;
                    this.NotifyOfPropertyChange(nameof(this.IgnoreCaseSensitive));
                    this.IsModified = true;
                }
                catch (Exception e)
                {
                    this.template.Dispatcher.Invoke(() =>
                    {
                        this.template.SetIgnoreCaseSensitive(this.authentication, originValue);
                    });
                    this.ignoreCaseSensitive = originValue;
                    this.NotifyOfPropertyChange(nameof(this.IgnoreCaseSensitive));
                    AppMessageBox.ShowError(e);
                }
            }
        }

        public override async void CanClose(Action<bool> callback)
        {
            if (this.template == null || this.IsModified == false)
            {
                callback(true);
                return;
            }

            var result = AppMessageBox.ConfirmSaveOnClosing();

            if (result == null)
                return;

            if (this.template != null && result == true)
            {
                this.BeginProgress(this.IsNew ? Resources.Message_Creating : Resources.Message_Changing);
                try
                {
                    await this.template.Dispatcher.InvokeAsync(() =>
                    {
                        this.template.EndEdit(this.authentication);
                        this.template.EditEnded -= Template_EditEnded;
                        this.template.EditCanceled -= Template_EditCanceled;
                        this.template.Changed -= Template_Changed;
                    });
                    this.template = null;
                    this.EndProgress();
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                    this.EndProgress();
                    return;
                }
            }

            this.DialogResult = result.Value;
            callback(true);
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanChange));
        }

        protected abstract void Verify(Action<bool> isValid);

        protected override async void OnInitialize()
        {
            base.OnInitialize();
            await this.template.Dispatcher.InvokeAsync(() =>
            {
                this.domain = this.template.Domain;
                this.tableName = this.template.TableName;
                this.comment = this.template.Comment;
                this.tags = this.template.Tags;
                this.selectableTypes = this.template.SelectableTypes;
                this.count = this.template.Count;
                this.source = this.domain.Source;
                this.isModified = this.template.IsModified;
                this.ignoreCaseSensitive = this.template.IgnoreCaseSensitive;
            });
            this.Refresh();
            this.Verify(this.VerifyAction);
        }

        protected override async void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            try
            {
                if (this.template != null)
                {
                    await this.template.Dispatcher.InvokeAsync(() =>
                    {
                        this.template.CancelEdit(this.authentication);
                    });
                }
            }
            catch (Exception e)
            {
                CremaLog.Error(e);
            }
            finally
            {
                if (this.template != null)
                {
                    await this.template.Dispatcher.InvokeAsync(() =>
                    {
                        this.template.EditEnded -= Template_EditEnded;
                        this.template.EditCanceled -= Template_EditCanceled;
                        this.template.Changed -= Template_Changed;
                        this.template = null;
                    });
                }
            }
        }

        protected override void OnCancel()
        {
            this.template = null;
            base.OnCancel();
        }

        private async void Template_EditEnded(object sender, EventArgs e)
        {
            if (e is DomainDeletedEventArgs ex)
            {
                this.template.EditEnded -= Template_EditEnded;
                this.template.EditCanceled -= Template_EditCanceled;
                this.template.Changed -= Template_Changed;
                this.template = null;

                await this.Dispatcher.InvokeAsync(() =>
                {
                    this.flashService?.Flash();
                    AppMessageBox.ShowInfo(Resources.Message_ExitEditByUser_Format, ex.UserID);
                    this.TryClose();
                });
            }
        }

        private async void Template_EditCanceled(object sender, EventArgs e)
        {
            if (e is DomainDeletedEventArgs ex)
            {
                this.template.EditEnded -= Template_EditEnded;
                this.template.EditCanceled -= Template_EditCanceled;
                this.template.Changed -= Template_Changed;
                this.template = null;

                await this.Dispatcher.InvokeAsync(() =>
                {
                    this.flashService?.Flash();
                    AppMessageBox.ShowInfo(Resources.Message_ExitEditByUser_Format, ex.UserID);
                    this.TryClose();
                });
            }
        }

        private void Template_Changed(object sender, EventArgs e)
        {
            this.isModified = this.template.IsModified;
            this.count = this.template.Count;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.Verify(this.VerifyAction);
            });
        }

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanChange));
        }
    }
}
