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
        [Route("tables/list")]
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
        [Route("tables/{tableName}/data")]
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
        [Route("tables/{tableName}/info")]
        public IDictionary<string, object> GetTableInfo(string databaseName, string tableName, string tags = null)
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
                    var props = new Dictionary<string, object>
                    {
                        { nameof(tableInfo.ID), tableInfo.ID },
                        { nameof(tableInfo.Name), tableInfo.Name },
                        { nameof(tableInfo.TableName), tableInfo.TableName },
                        { nameof(tableInfo.Tags), $"{tableInfo.Tags}" },
                        { nameof(tableInfo.DerivedTags), $"{tableInfo.DerivedTags}" },
                        { nameof(tableInfo.Comment), tableInfo.Comment },
                        { nameof(tableInfo.TemplatedParent), tableInfo.TemplatedParent },
                        { nameof(tableInfo.ParentName), tableInfo.ParentName },
                        { nameof(tableInfo.CategoryPath), tableInfo.CategoryPath },
                        { nameof(tableInfo.HashValue), tableInfo.HashValue },
                        { CremaSchema.Creator, tableInfo.CreationInfo.ID },
                        { CremaSchema.CreatedDateTime, tableInfo.CreationInfo.DateTime },
                        { CremaSchema.Modifier, tableInfo.ModificationInfo.ID },
                        { CremaSchema.ModifiedDateTime, tableInfo.ModificationInfo.DateTime },
                        { CremaSchema.ContentsModifier, tableInfo.ContentsInfo.ID },
                        { CremaSchema.ContentsModifiedDateTime, tableInfo.ContentsInfo.DateTime },
                        { nameof(tableInfo.Columns), this.GetColumnsInfo(tableInfo.Columns) }
                    };

                    return props;
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

        [HttpGet]
        [Route("tables/{tableName}/delete")]
        public void CopyTable(string databaseName, string tableName)
        {
            var table = this.GetTable(databaseName, tableName);
            table.Dispatcher.Invoke(() => table.Delete(this.Authentication));
        }

        [HttpPost]
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

        [HttpPost]
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
        [Route("table-item/list")]
        public string[] GetTableItemList(string databaseName)
        {
            var dataBase = this.GetDataBase(databaseName);
            return dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Select(item => item.Path).ToArray());
        }

        [HttpPost]
        [Route("table-item/log")]
        public IDictionary<string, object>[] GetTableItemLogInfo(string databaseName, [FromBody] GetTableItemLogInfoRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            return tableItem.Dispatcher.Invoke(() =>
            {
                var logInfos = tableItem.GetLog(this.Authentication);
                return this.GetLogInfo(logInfos);
            });
        }

        [HttpPost]
        [Route("table-item/move")]
        public void MoveTableItem(string databaseName, [FromBody] MoveTableItemRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            tableItem.Dispatcher.Invoke(() => tableItem.Move(this.Authentication, request.ParentPath));
        }

        [HttpPost]
        [Route("table-item/rename")]
        public void RenameTableItem(string databaseName, [FromBody] RenameTableItemRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            tableItem.Dispatcher.Invoke(() => tableItem.Rename(this.Authentication, request.NewName));
        }

        [HttpPost]
        [Route("table-item/delete")]
        public void DeleteTableItem(string databaseName, [FromBody] DeleteTableItemRequest request)
        {
            var tableItem = this.GetTableItem(databaseName, request.TableItemPath);
            tableItem.Dispatcher.Invoke(() => tableItem.Delete(this.Authentication));
        }

        [HttpPost]
        [Route("table-item/contains")]
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

        private object[] GetColumnsInfo(ColumnInfo[] columns)
        {
            var props = new object[columns.Length];
            for (var i = 0; i < columns.Length; i++)
            {
                props[i] = this.GetColumnInfo(columns[i]);
            }
            return props;
        }

        private IDictionary<string, object> GetColumnInfo(ColumnInfo columnInfo)
        {
            var props = new Dictionary<string, object>
            {
                { nameof(columnInfo.ID), columnInfo.ID },
                { nameof(columnInfo.IsKey), columnInfo.IsKey },
                { nameof(columnInfo.IsUnique), columnInfo.IsUnique },
                { nameof(columnInfo.AllowNull), columnInfo.AllowNull },
                { nameof(columnInfo.Name), columnInfo.Name },
                { nameof(columnInfo.DataType), columnInfo.DataType },
                { nameof(columnInfo.DefaultValue), this.GetDefaultValue(columnInfo) },
                { nameof(columnInfo.Comment), columnInfo.Comment },
                { nameof(columnInfo.AutoIncrement), columnInfo.AutoIncrement },
                { nameof(columnInfo.ReadOnly), columnInfo.ReadOnly },
                { nameof(columnInfo.Tags), $"{columnInfo.Tags}" },
                { nameof(columnInfo.DerivedTags), $"{columnInfo.DerivedTags}" },
                { CremaSchema.Creator, columnInfo.CreationInfo.ID },
                { CremaSchema.CreatedDateTime, columnInfo.CreationInfo.DateTime },
                { CremaSchema.Modifier, columnInfo.ModificationInfo.ID },
                { CremaSchema.ModifiedDateTime, columnInfo.ModificationInfo.DateTime }
            };
            return props;
        }

        private object GetDefaultValue(ColumnInfo columnInfo)
        {
            if (columnInfo.DefaultValue != null && CremaDataTypeUtility.IsBaseType(columnInfo.DataType) == true)
            {
                var type = CremaDataTypeUtility.GetType(columnInfo.DataType);
                return CremaConvert.ChangeType(columnInfo.DefaultValue, type);
            }
            return columnInfo.DefaultValue;
        }

        private IDictionary<string, object> GetLogInfo(LogInfo columnInfo)
        {
            var props = new Dictionary<string, object>
            {
                { nameof(columnInfo.UserID), columnInfo.UserID },
                { nameof(columnInfo.Revision), columnInfo.Revision },
                { nameof(columnInfo.Comment), columnInfo.Comment },
                { nameof(columnInfo.DateTime), columnInfo.DateTime }
            };
            return props;
        }

        private IDictionary<string, object>[] GetLogInfo(LogInfo[] logInfos)
        {
            var props = new IDictionary<string, object>[logInfos.Length];
            for (var i = 0; i < logInfos.Length; i++)
            {
                props[i] = this.GetLogInfo(logInfos[i]);
            }
            return props;
        }
    }
}
