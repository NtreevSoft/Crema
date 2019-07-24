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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using Ntreev.ModernUI.Framework.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Client.Framework.Controls
{
    public class DomainDataRow : ModernDataRow
    {
        private readonly DomainDataUserCollection userInfos = new DomainDataUserCollection();

        private IDomain domain;
        private object[] keys;
        private string tableName;
        private string userID;

        public DomainDataRow()
        {
            this.CommandBindings.Insert(0, new CommandBinding(ApplicationCommands.Delete, this.Delete_Execute, this.Delete_CanExecute));
        }

        public void Delete()
        {
            var domain = this.GridControl.Domain;
            var authenticator = domain.GetService(typeof(Authenticator)) as Authenticator;
            var item = this.DataContext;
            domain.Dispatcher.Invoke(() => domain.RemoveRow(authenticator, item));
        }

        public new DomainDataGridControl GridControl
        {
            get { return (DomainDataGridControl)base.GridControl; }
        }

        public bool CanDelete
        {
            get
            {
                if (this.ReadOnly == true)
                    return false;

                if (this.GridControl.SelectedContexts.Count > 1)
                    return false;

                if (this.DataContext is System.Data.DataRowView == false)
                    return false;

                if (this.IsBeingEdited == true)
                    return false;

                var index = this.GridContext.Items.IndexOf(this.DataContext);

                foreach (var item in this.GridContext.SelectedCellRanges)
                {
                    if (item.ItemRange.Length != 1)
                        return false;
                    if (item.ItemRange.StartIndex != index)
                        return false;
                }

                if (this.GridContext.GetSelectedColumns().Count() != this.GridContext.VisibleColumns.Count)
                    return false;

                return true;
            }
        }

        // null 예외 발생함. 연속으로
        protected async override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            base.PrepareContainer(dataGridContext, item);
            var gridControl = dataGridContext.DataGridControl as DomainDataGridControl;

            if (this.domain == null)
            {
                var domain = gridControl.Domain;
                if (domain != null)
                {
                    await domain.Dispatcher.InvokeAsync(() =>
                    {
                        this.domain = domain;
                        this.domain.UserChanged += Domain_UserChanged;
                        this.domain.UserRemoved += Domain_Disjoined;
                        this.domain.Deleted += Domain_Deleted;
                        if (this.domain.GetService(typeof(ICremaHost)) is ICremaHost cremaHost)
                        {
                            this.userID = cremaHost.UserID;
                            this.UserToken = cremaHost.Token;
                        }
                    });
                }
            }

            this.userInfos.Clear();

            if (this.domain != null)
            {
                var domain = this.domain;
                var infos = await domain.Dispatcher.InvokeAsync(() => domain.Users.Select(i => new DomainUserMetaData()
                {
                    DomainUserInfo = i.DomainUserInfo,
                    DomainUserState = i.DomainUserState,
                }).ToArray());

                foreach (var i in infos)
                {
                    if (HashUtility.Equals(this.keys, i.DomainUserInfo.Location.Keys) == true && this.tableName == i.DomainUserInfo.Location.TableName)
                    {
                        this.userInfos.Set(i.DomainUserInfo, i.DomainUserState);
                    }
                }
            }
        }

        protected override void SetDataContext(object item)
        {
            var changed = this.DataContext != item;
            base.SetDataContext(item);
            this.keys = CremaDataRowUtility.GetKeys(item);
            this.tableName = CremaDataRowUtility.GetTableName(item);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override Cell CreateCell(ColumnBase column)
        {
            return new DomainDataCell();
        }

        protected override void EndEditCore()
        {
            try
            {
                this.IsBeginEnding = true;
                base.EndEditCore();
            }
            finally
            {
                this.IsBeginEnding = false;
            }
        }

        protected override void CancelEditCore()
        {
            try
            {
                this.IsBeginEnding = true;
                base.CancelEditCore();
            }
            finally
            {
                this.IsBeginEnding = false;
            }
        }

        private async void Domain_UserChanged(object sender, DomainUserEventArgs e)
        {
            var domainUserInfo = e.DomainUserInfo;
            var domainUserState = e.DomainUserState;
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.DataContext == null)
                    return;

                if (HashUtility.Equals(this.keys, domainUserInfo.Location.Keys) == true && this.tableName == domainUserInfo.Location.TableName)
                {
                    this.userInfos.Set(domainUserInfo, domainUserState);
                }
                else
                {
                    this.userInfos.Remove(domainUserInfo.Token);
                }
            });
        }

        private async void Domain_Disjoined(object sender, DomainUserEventArgs e)
        {
            var domainUserInfo = e.DomainUserInfo;
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.userInfos.Remove(domainUserInfo.Token);
            });
        }

        private void Domain_Deleted(object sender, EventArgs e)
        {
            this.domain.UserChanged -= Domain_UserChanged;
            this.domain.UserRemoved -= Domain_Disjoined;
            this.domain.Deleted -= Domain_Deleted;
            this.domain = null;
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CanDelete;
        }

        private void Delete_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (AppMessageBox.ShowQuestion(Properties.Resources.Message_ConfirmToDeleteRow) == false)
                return;

            try
            {
                this.Delete();
            }
            catch (Exception ex)
            {
                AppMessageBox.ShowError(ex);
            }
        }

        internal bool IsBeginEnding
        {
            get;
            set;
        }

        internal DomainDataUserCollection UserInfos
        {
            get { return this.userInfos; }
        }

        internal string UserID
        {
            get { return this.userID; }
        }

        internal Guid UserToken { get; private set; }
    }
}
