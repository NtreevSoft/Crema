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
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Requests.Commands;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands/databases/{databaseName}")]
    public class TablesApiController : CremaApiController
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TablesApiController(ICremaHost cremaHost) : base(cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [HttpGet]
        [Route("tables")]
        public string[] GetTableList(string databaseName, string tags = null)
        {
            var dataBase = this.GetDataBase(databaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(tags))
                {
                    var tables = dataBase.TableContext.Tables;
                    var query = from item in tables
                        where (item.TableInfo.DerivedTags & (TagInfo)tags) != TagInfo.Unused
                        select item.Name;
                    return query.ToArray();
                }

                return dataBase.TableContext.Select(item => item.Name).ToArray();
            });
        }

        [HttpGet]
        [Route("tables/{tableName}")]
        public IDictionary<int, object> GetTableData(string databaseName, string tableName, int revision = -1, string tags = null)
        {
            var table = this.GetTable(databaseName, tableName);

            return table.Dispatcher.Invoke(() =>
            {
                var dataSet = table.GetDataSet(this.Authentication, revision);
                if (!string.IsNullOrWhiteSpace(tags))
                {
                    dataSet = dataSet.Filter(tableName, tags);
                }
                var dataTable = dataSet.Tables[tableName];
                return this.GetDataRows(dataTable);
            });
        }

        [HttpGet]
        [Route("tables/*/info")]
        public GetTableInfoResponse[] GetTablesInfo(string databaseName, string tags = null, string columnTags = null)
        {
            var tables = this.GetTables(databaseName, tags);
            return tables.Select(table =>
            {
                return table.Dispatcher.Invoke(() =>
                {
                    var tableInfo = table.TableInfo;
                    if (!string.IsNullOrWhiteSpace(columnTags))
                    {
                        tableInfo = table.TableInfo.Filter((TagInfo)columnTags);
                    }

                    return GetTableInfoResponse.ConvertFrom(tableInfo);
                });
            }).ToArray();
        }

        [HttpPost]
        [Route("tables/*/info")]
        public GetTableInfoResponse[] GetTablesInfoByTableName(string databaseName, [FromBody] GetTableInfoByTableNameRequest request, string tags = null, string columnTags = null)
        {
            var tables = this.GetTables(databaseName, tags);
            var intersectTables = tables.Select(table => table.Dispatcher.Invoke(() => table.TableName))
                .Intersect(request.TableNames, StringComparer.OrdinalIgnoreCase);

            return intersectTables.Select(tableName =>
            {
                var table = tables.First(o => o.Dispatcher.Invoke(() => o.TableName == tableName));
                var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
                if (!string.IsNullOrWhiteSpace(columnTags))
                {
                    tableInfo = tableInfo.Filter((TagInfo)columnTags);
                }
                return GetTableInfoResponse.ConvertFrom(tableInfo);
            }).ToArray();
        }

        [HttpGet]
        [Route("tables/{tableName}/info")]
        public GetTableInfoResponse GetTableInfo(string databaseName, string tableName, string tags = null)
        {
                var table = this.GetTable(databaseName, tableName);
                return table.Dispatcher.Invoke(() =>
                {
                    return table.Dispatcher.Invoke(() =>
                    {
                        var tableInfo = table.TableInfo;
                        if (!string.IsNullOrWhiteSpace(tags))
                        {
                            tableInfo = table.TableInfo.Filter((TagInfo)tags);
                        }

                        return GetTableInfoResponse.ConvertFrom(tableInfo);
                    });
                });
        }

        [HttpPost]
        [Route("tables/{tableName}/copy")]
        public void CopyTable(string databaseName, string tableName, [FromBody] CopyTableRequest request)
        {
            var table = this.GetTable(databaseName, tableName);
            table.Dispatcher.Invoke(() => table.Copy(this.Authentication, request.NewTableName,request.CategoryPath, request.CopyContent));
        }

        [HttpDelete]
        [Route("tables/{tableName}")]
        public void DeleteTable(string databaseName, string tableName)
        {
            var table = this.GetTable(databaseName, tableName);
            table.Dispatcher.Invoke(() => table.Delete(this.Authentication));
        }

        [HttpPut]
        [Route("tables/{tableName}/move")]
        public void MoveTable(string databaseName, string tableName, [FromBody] MoveTableRequest request)
        {
            var table = this.GetTable(databaseName, tableName);
            table.Dispatcher.Invoke(() => table.Move(this.Authentication, request.CategoryPath));
        }

        [HttpPost]
        [Route("tables/{tableName}/inherit")]
        public void InheritTable(string databaseName, string tableName, [FromBody] InheritTableRequest request)
        {
            var table = this.GetTable(databaseName, tableName);
            table.Dispatcher.Invoke(() => table.Inherit(this.Authentication, request.NewTableName, request.CategoryPath, request.CopyContent));
        }

        [HttpPut]
        [Route("tables/{tableName}/rename")]
        public void RenameTable(string databaseName, string tableName, [FromBody] RenameTableRequest request)
        {
            var table = this.GetTable(databaseName, tableName);
            table.Dispatcher.Invoke(() => table.Rename(this.Authentication, request.NewTableName));
        }

        [HttpGet]
        [Route("tables/{tableName}/contains")]
        public ContainsTableResponse ContainsTable(string databaseName, string tableName)
        {
            var dataBase = this.GetDataBase(databaseName);
            var contains = dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Tables.Contains(tableName));
            return new ContainsTableResponse
            {
                Contains = contains
            };
        }

        [HttpGet]
        [Route("table-items")]
        public string[] GetTableItemList(string databaseName)
        {
            var dataBase = this.GetDataBase(databaseName);
            return dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Select(item => item.Path).ToArray());
        }

        [HttpPost]
        [Route("table-items/log")]
        public GetTableItemLogInfoResponse[] GetTableItemLogInfo(string databaseName, [FromBody] GetTableItemLogInfoRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            return tableItem.Dispatcher.Invoke(() =>
            {
                var logInfos = tableItem.GetLog(this.Authentication);
                return logInfos.Select(GetTableItemLogInfoResponse.ConvertFrom).ToArray();
            });
        }

        [HttpPut]
        [Route("table-items/move")]
        public void MoveTableItem(string databaseName, [FromBody] MoveTableItemRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            tableItem.Dispatcher.Invoke(() => tableItem.Move(this.Authentication, request.ParentPath));
        }

        [HttpPut]
        [Route("table-items/rename")]
        public void RenameTableItem(string databaseName, [FromBody] RenameTableItemRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            tableItem.Dispatcher.Invoke(() => tableItem.Rename(this.Authentication, request.NewName));
        }

        [HttpPut]
        [Route("table-items/delete")]
        public void DeleteTableItem(string databaseName, [FromBody] DeleteTableItemRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            tableItem.Dispatcher.Invoke(() => tableItem.Delete(this.Authentication));
        }

        [HttpPost]
        [Route("table-items/contains")]
        public ContainsTableItemResponse ContainsTableItem(string databaseName, [FromBody] ContainsTableItemRequest request)
        {
            var dataBase = this.GetDataBase(databaseName);
            var contains = dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Contains(request.TableItemPath));
            return new ContainsTableItemResponse
            {
                Contains = contains
            };
        }

        private IDataBase GetDataBase(string databaseName)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));
            return this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(databaseName) == false)
                    throw new DataBaseNotFoundException(databaseName);
                return this.cremaHost.DataBases[databaseName];
            });
        }

        private ITable[] GetTables(string dataBaseName, string tags = null)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));

            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                if (tags == null)
                {
                    var tables = dataBase.TableContext.Tables.ToArray();
                    return tables;
                }
                else
                {
                    var tagInfo = (TagInfo)tags;
                    var tables = dataBase.TableContext.Tables.Where(table => (table.TableInfo.DerivedTags & tagInfo) != TagInfo.Unused).ToArray();
                    return tables;
                }
            });
        }

        private ITable GetTable(string dataBaseName, string tableName)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                if (table == null)
                    throw new TableNotFoundException(tableName);
                return table;
            });
        }

        protected ITableItem GetTableItem(string dataBaseName, string tableItemPath)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));
            if (tableItemPath == null)
                throw new ArgumentNullException(nameof(tableItemPath));
            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                var tableItem = dataBase.TableContext[tableItemPath];
                if (tableItem == null)
                    throw new ItemNotFoundException(tableItemPath);
                return tableItem;
            });
        }

        private IDictionary<int, object> GetDataRows(CremaDataTable dataTable)
        {
            var props = new Dictionary<int, object>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var dataRow = dataTable.Rows[i];
                props.Add(i, this.GetDataRow(dataRow));
            }
            return props;
        }

        private IDictionary<string, object> GetDataRow(CremaDataRow dataRow)
        {
            var dataTable = dataRow.Table;
            var props = new Dictionary<string, object>();
            foreach (var item in dataTable.Columns)
            {
                var value = dataRow[item];
                if (value == DBNull.Value)
                    props.Add(item.ColumnName, null);
                else
                    props.Add(item.ColumnName, value);
            }
            if (dataRow.ParentID != null)
                props.Add(CremaSchema.__ParentID__, dataRow.ParentID);
            if (dataRow.RelationID != null)
                props.Add(CremaSchema.__RelationID__, dataRow.RelationID);
            return props;
        }
    }
}
