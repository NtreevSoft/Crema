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
using Ntreev.Crema.Designer.Properties;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Designer.Tables.ViewModels
{
    public abstract class TemplateViewModel : ModalDialogBase
    {
        private CremaTemplate template;
        private bool isNew;
        private bool isReadOnly;
        private bool isModified;
        private bool isValid;
        private string tableName;
        private string comment;
        private TagInfo tags;
        private string[] selectableTypes;

        protected TemplateViewModel(CremaTemplate template)
            : this(template, false)
        {

        }

        protected TemplateViewModel(CremaTemplate template, bool isNew)
        {
            this.DisplayName = Resources.Title_EditTableTemplate;
            this.isNew = isNew;
            this.template = template;
            this.Initialize();
        }

        public void Change()
        {
            try
            {
                this.BeginProgress("저장중입니다.");
                this.template = null;
                this.isModified = false;
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e.Message);
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

        public CremaTemplate Template
        {
            get { return this.template; }
        }

        public object Source
        {
            get { return this.template; }
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

        public string TableName
        {
            get { return this.tableName ?? string.Empty; }
            set
            {
                this.template.TableName = value;
                this.tableName = value;
                this.NotifyOfPropertyChange(nameof(this.TableName));
                this.Verify(this.VerifyAction);
            }
        }

        public string Comment
        {
            get { return this.comment ?? string.Empty; }
            set
            {
                this.template.Comment = value;
                this.comment = value;
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
                if (this.template.Columns.Count == 0)
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
                this.template.Tags = value;
                this.tags = value;
                this.NotifyOfPropertyChange(nameof(this.Tags));
            }
        }

        public override void CanClose(Action<bool> callback)
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
                this.BeginProgress("저장중입니다.");
                try
                {
                    //await this.template.Dispatcher.InvokeAsync(() =>
                    //{
                    //    this.domain.Deleted -= Domain_Deleted;
                    //    this.dataBase.Unloaded -= DataBase_Unloaded;
                    //    this.template.EndEdit(this.authentication);
                    //});
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

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            //await this.dataBase.Dispatcher.InvokeAsync(() =>
            //{
            //    this.dataBase.Unloaded -= DataBase_Unloaded;
            //    if (this.template != null)
            //    {
            //        this.domain.Deleted -= Domain_Deleted;
            //        this.template.CancelEdit(this.authentication);
            //    }
            //});
            this.template = null;

            //this.cremaAppHost.Unloaded -= CremaAppHost_Unloaded;
        }

        private void Initialize()
        {
            this.tableName = this.template.TableName;
            this.comment = this.template.Comment;
            this.tags = this.template.Tags;
            this.selectableTypes = this.template.Types;
            
            //this.template.ColumnAdded += Template_ColumnAdded;
            this.template.ColumnChanged += Template_ColumnChanged;
            this.Refresh();
            this.Verify(this.VerifyAction);
        }

        private void Template_ColumnAdded(object sender, CremaTemplateColumnChangeEventArgs e)
        {
            this.IsModified = true;
        }

        private void Template_ColumnChanged(object sender, CremaTemplateColumnChangeEventArgs e)
        {
            this.IsModified = true;
        }

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanChange));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual string Button_Change
        {
            get { return Resources.Button_Change; }
        }
    }
}
