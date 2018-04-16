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

namespace Ntreev.Crema.Designer.Types.ViewModels
{
    public abstract class TemplateViewModel : ModalDialogBase
    {
        private CremaDataType dataType;
        private bool isNew;
        private bool isReadOnly;
        private bool isModified;
        private bool isValid;
        private string typeName;
        private string comment;
        private bool isFlag;
        private int count;

        protected TemplateViewModel(CremaDataType dataType)
            : this(dataType, false)
        {

        }

        protected TemplateViewModel(CremaDataType dataType, bool isNew)
        {
            this.isNew = isNew;
            this.dataType = dataType;
            this.typeName = dataType.TypeName;
            this.count = 0;
            this.DisplayName = Resources.Title_EditTypeTemplate;
        }

        public void Change()
        {
            try
            {
                this.BeginProgress(Resources.Progress_Change);
                //await this.template.Dispatcher.InvokeAsync(() =>
                //{
                //    this.dataBase.Unloaded -= DataBase_Unloaded;
                //    this.domain.Deleted -= Domain_Deleted;
                //    this.template.EndEdit(this.authentication);
                //});
                //this.domain = null;
                //this.template = null;
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

        public object Source
        {
            get { return this.dataType; }
        }

        public string TypeName
        {
            get { return this.typeName ?? string.Empty; }
            set
            {
                this.typeName = value;
                this.NotifyOfPropertyChange(nameof(this.TypeName));
                this.Verify(this.VerifyAction);
            }
        }

        public string Comment
        {
            get { return this.comment ?? string.Empty; }
            set
            {
                this.comment = value;
                this.NotifyOfPropertyChange(nameof(this.Comment));
            }
        }

        public bool CanSave
        {
            get
            {
                if (this.IsProgressing == true || this.isReadOnly == true)
                    return false;
                //if (this.template == null)
                //    return false;
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
                this.NotifyOfPropertyChange(nameof(this.CanSave));
            }
        }

        public bool IsFlag
        {
            get { return this.isFlag; }
            set
            {
                this.isFlag = value;
                this.NotifyOfPropertyChange(nameof(this.IsFlag));
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            if (this.IsModified == false)
            {
                callback(true);
                return;
            }

            var result = AppMessageBox.ConfirmSaveOnClosing();

            if (result == null)
                return;

            //if (this.template != null && result == true)
            //{
            //    this.BeginProgress(Resources.Progress_Save);
            //    try
            //    {
            //        await this.template.Dispatcher.InvokeAsync(() =>
            //        {
            //            this.domain.Deleted -= Domain_Deleted;
            //            this.template.EndEdit(this.authentication);
            //        });
            //        this.template = null;
            //        this.EndProgress();
            //    }
            //    catch (Exception e)
            //    {
            //        AppMessageBox.ShowError(e);
            //        this.EndProgress();
            //        return;
            //    }
            //}

            this.DialogResult = result.Value;
            callback(true);
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanSave));
        }

        protected abstract void Verify(Action<bool> isValid);

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanSave));
        }

        #region localization

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual string Button_Change => Resources.Button_Change;

        #endregion
    }
}
