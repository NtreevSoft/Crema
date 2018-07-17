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
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Tables.Properties;
using System.Windows;
using System.Collections;
using System.Threading;
using Ntreev.ModernUI.Framework;
using Ntreev.Library.Linq;
using Ntreev.Crema.ServiceModel;
using System.Collections.Generic;

namespace Ntreev.Crema.Client.Tables.Documents.ViewModels
{
    class TableViewerViewModel : TableDocumentBase
    {
        private readonly Authentication authentication;
        private readonly TableDescriptor descriptor;
        private readonly TableContentDescriptor contentDescriptor;

        public TableViewerViewModel(Authentication authentication, TableDescriptor descriptor)
        {
            this.authentication = authentication;
            this.authentication.Expired += (s, e) => this.Dispatcher.InvokeAsync(() => this.Tables.Clear());
            this.descriptor = descriptor;
            this.contentDescriptor = descriptor.ContentDescriptor;
            this.AttachEvent();
            this.DisplayName = descriptor.DisplayName;
            this.Target = descriptor.Target;
            foreach (var item in EnumerableUtility.FamilyTree(this.descriptor, item => item.Childs))
            {
                this.Tables.Add(new TableItemViewModel(this.authentication, item, this));
            }
            this.Initialize();
        }

        protected override Task CloseAsync()
        {
            if (this.Tables.Any() == true)
            {
                this.DetachEvent();
            }
            return base.CloseAsync();
        }

        private void ContentDescriptor_EditEnded(object sender, EventArgs e)
        {
            //this.DetachEvent();
            //this.tableItems = null;
            //if (e is DomainDeletedEventArgs ex)
            //{
            //    this.flashServie?.Flash();
            //    AppMessageBox.ShowInfo("'{0}'에 의해서 편집이 종료되었습니다.", ex.UserID);
            //}
            //this.TryClose();
        }

        private void ContentDescriptor_EditCanceled(object sender, EventArgs e)
        {
            //this.DetachEvent();
            //this.tableItems = null;
            //if (e is DomainDeletedEventArgs ex)
            //{
            //    this.flashServie?.Flash();
            //    AppMessageBox.ShowInfo("'{0}'에 의해서 편집이 취소되었습니다.", ex.UserID);
            //}
            //this.TryClose();
        }

        private void ContentDescriptor_Kicked(object sender, EventArgs e)
        {
            //this.DetachEvent();
            //this.tableItems = null;
            //this.flashServie?.Flash();
            //if (e is DomainUserRemovedEventArgs ex)
            //{
            //    AppMessageBox.ShowInfo(ex.RemoveInfo.Message, "추방되었습니다.");
            //}
            //this.TryClose();
        }

        private async void Initialize()
        {
            try
            {
                this.BeginProgress(Resources.Message_LoadingData);


                var dataSet = await TableDescriptorUtility.GetDataAsync(this.authentication, this.descriptor, null);
                foreach (var item in this.Tables)
                {
                    item.Source = dataSet.Tables[item.Name];
                }
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                this.EndProgress();
                this.DetachEvent();
                this.Tables.Clear();
                this.TryClose();
                return;
            }

            //if (this.selectedItem == null this.selectedTableName != null)
            //    this.SelectedItem = this.tableItems.Where(item => item.DisplayName == this.selectedTableName).First();
            //else
            //    this.SelectedItem = this.tableItems.First();

            //this.selectedTableName = null;

            this.EndProgress();
            this.NotifyOfPropertyChange(nameof(this.Tables));
            this.NotifyOfPropertyChange(nameof(this.SelectedTable));
            this.NotifyOfPropertyChange(nameof(this.IsProgressing));
        }

        private void AttachEvent()
        {
            this.contentDescriptor.EditEnded += ContentDescriptor_EditEnded;
            this.contentDescriptor.EditCanceled += ContentDescriptor_EditCanceled;
            this.contentDescriptor.Kicked += ContentDescriptor_Kicked;
        }

        private void DetachEvent()
        {
            this.contentDescriptor.EditEnded -= ContentDescriptor_EditEnded;
            this.contentDescriptor.EditCanceled -= ContentDescriptor_EditCanceled;
            this.contentDescriptor.Kicked -= ContentDescriptor_Kicked;
        }
    }
}
