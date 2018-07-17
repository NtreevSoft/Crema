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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Domains;
using Ntreev.Library;

namespace Ntreev.Crema.Services.Data
{
    partial class TableContent
    {
        private TableContentDomainHost domainHost;

        public class TableContentDomainHost : IDomainHost
        {
            private readonly TableCollection container;
            private readonly TableContent[] contents;
            private readonly Table[] tables;
            private Domain domain;

            private string masterUserID;

            public TableContentDomainHost(TableCollection container, Domain domain, string itemPath)
            {
                var items = StringUtility.Split(itemPath, '|');
                var tableList = new List<Table>(items.Length);
                var dataBase = container.DataBase;
                foreach (var item in items)
                {
                    if (dataBase.TableContext[item] is Table table)
                    {
                        tableList.Add(table);
                    }
                }

                this.container = container;
                this.tables = tableList.ToArray();
                this.contents = tableList.Select(item => item.Content).ToArray();
                this.domain = domain;
                foreach (var item in this.contents)
                {
                    item.domainHost = this;
                }
            }

            public void AttachDomainEvent()
            {
                this.domain.Deleted += Domain_Deleted;
                this.domain.RowAdded += Domain_RowAdded;
                this.domain.RowChanged += Domain_RowChanged;
                this.domain.RowRemoved += Domain_RowRemoved;
                this.domain.PropertyChanged += Domain_PropertyChanged;
                this.domain.UserChanged += Domain_UserChanged;
            }

            public void DetachDomainEvent()
            {
                this.domain.Deleted -= Domain_Deleted;
                this.domain.RowAdded -= Domain_RowAdded;
                this.domain.RowChanged -= Domain_RowChanged;
                this.domain.RowRemoved -= Domain_RowRemoved;
                this.domain.PropertyChanged -= Domain_PropertyChanged;
                this.domain.UserChanged -= Domain_UserChanged;
            }

            public void InvokeEditBegunEvent(EventArgs e)
            {
                foreach (var item in this.contents)
                {
                    item.OnEditBegun(e);
                }
            }

            public void InvokeEditEndedEvent(EventArgs e)
            {
                foreach (var item in this.contents)
                {
                    item.OnEditEnded(e);
                }
            }

            public void InvokeEditCanceledEvent(EventArgs e)
            {
                foreach (var item in this.contents)
                {
                    item.OnEditCanceled(e);
                }
            }

            public void BeginContent(Authentication authentication, Domain domain)
            {
                foreach (var item in this.contents)
                {
                    item.domain = domain;
                    item.table.SetTableState(TableState.IsBeingEdited);
                }
                this.container.InvokeTablesStateChangedEvent(authentication, this.tables);
            }

            public void EndContent(Authentication authentication, TableInfo[] tableInfos)
            {
                foreach (var item in this.contents)
                {
                    item.domain = null;
                    item.isModified = false;
                    item.dataTable = null;
                    if (tableInfos.Any(i => i.Name == item.table.Name))
                        item.table.UpdateContent(tableInfos.First(i => i.Name == item.table.Name));
                    item.table.SetTableState(TableState.None);
                }
                this.container.InvokeTablesContentChangedEvent(authentication, this.tables);
                this.container.InvokeTablesStateChangedEvent(authentication, this.tables);
            }

            public void CancelContent(Authentication authentication)
            {
                foreach (var item in this.contents)
                {
                    item.domain = null;
                    item.isModified = false;
                    item.dataTable = null;
                    item.table.SetTableState(TableState.None);
                }
                this.container.InvokeTablesStateChangedEvent(authentication, this.tables);
            }

            public void EnterContent(Authentication authentication, Domain domain)
            {
                var dataSet = domain.Source as CremaDataSet;
                foreach (var item in this.contents)
                {
                    var tableState = item.table.TableState;
                    item.dataTable = dataSet?.Tables[item.table.Name, item.table.Category.Path];
                    if (dataSet != null)
                        tableState |= TableState.IsMember;
                    if (this.masterUserID == authentication.ID)
                        tableState |= TableState.IsOwner;
                    item.table.SetTableState(tableState);
                }

                this.container.InvokeTablesStateChangedEvent(authentication, this.tables);
            }

            public void LeaveContent(Authentication authentication)
            {
                foreach (var item in this.contents)
                {
                    item.dataTable = null;
                    item.table.SetTableState(item.table.TableState & ~TableState.IsMember);
                }

                this.container.InvokeTablesStateChangedEvent(authentication, this.tables);
            }

            public Table[] Tables => this.tables;

            private void Domain_Deleted(object sender, DomainDeletedEventArgs e)
            {
                var isCanceled = e.IsCanceled;
                this.Dispatcher?.InvokeAsync(() =>
                {
                    if (isCanceled == false)
                    {
                        this.EndContent(e.Authentication, new TableInfo[] { });
                        this.InvokeEditEndedEvent(e);
                    }
                    else
                    {
                        this.CancelContent(e.Authentication);
                        this.InvokeEditCanceledEvent(e);
                    }
                });
            }

            private void Domain_RowAdded(object sender, DomainRowEventArgs e)
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    var query = from row in e.Rows
                                join content in this.contents on row.TableName equals content.dataTable.Name
                                select content;
                    foreach (var item in query)
                    {
                        item.isModified = true;
                        item.OnChanged(e);
                    }
                });
            }

            private void Domain_RowChanged(object sender, DomainRowEventArgs e)
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    var query = from row in e.Rows
                                join content in this.contents on row.TableName equals content.dataTable.Name
                                select content;
                    foreach (var item in query)
                    {
                        item.isModified = true;
                        item.OnChanged(e);
                    }
                });
            }

            private void Domain_RowRemoved(object sender, DomainRowEventArgs e)
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    var query = from row in e.Rows
                                join content in this.contents on row.TableName equals content.dataTable.Name
                                select content;
                    foreach (var item in query)
                    {
                        item.isModified = true;
                        item.OnChanged(e);
                    }
                });
            }

            private void Domain_PropertyChanged(object sender, DomainPropertyEventArgs e)
            {

            }

            private void Domain_UserChanged(object sender, DomainUserEventArgs e)
            {
                if (this.masterUserID == this.domain.Users.OwnerUserID)
                    return;

                this.masterUserID = this.domain.Users.OwnerUserID;
                foreach (var item in this.contents)
                {
                    var tableState = item.table.TableState;
                    if (this.masterUserID == this.domain.CremaHost.UserID)
                        tableState |= TableState.IsOwner;
                    else
                        tableState &= ~TableState.IsOwner;
                    item.table.SetTableState(tableState);
                }
                Authentication.System.Sign();
                this.container.InvokeTablesStateChangedEvent(Authentication.System, this.tables);
            }

            private CremaDispatcher Dispatcher => this.container.Dispatcher;

            #region IDomainHost

            void IDomainHost.Detach()
            {
                this.domain.Dispatcher.Invoke(this.DetachDomainEvent);
                this.domain = null;
                foreach (var item in this.contents)
                {
                    item.domain = null;
                    item.dataTable = null;
                }
            }

            void IDomainHost.Restore(Authentication authentication, Domain domain)
            {
                var dataSet = domain.Source as CremaDataSet;
                this.domain = domain;
                this.masterUserID = this.domain.Users.OwnerUserID;
                foreach (var item in this.contents)
                {
                    var tableState = TableState.IsBeingEdited;
                    item.domainHost = this;
                    item.domain = domain;
                    if (dataSet != null)
                    {
                        item.dataTable = dataSet.Tables[item.table.Name, item.table.Category.Path];
                        if (dataSet != null)
                            tableState |= TableState.IsMember;
                        if (this.masterUserID == authentication.ID)
                            tableState |= TableState.IsOwner;
                    }
                    item.table.SetTableState(tableState);
                    item.isModified = domain.ModifiedTables.Contains(item.table.Name);
                }
                this.domain.Dispatcher.Invoke(this.AttachDomainEvent);
                this.container.InvokeTablesStateChangedEvent(authentication, this.tables);
                this.InvokeEditBegunEvent(EventArgs.Empty);
            }

            #endregion
        }
    }
}
