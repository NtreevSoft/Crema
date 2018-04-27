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
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.ComponentModel;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Javascript.Methods.DataBase
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(DataBase))]
    class GetTableInfoMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableInfoMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, IDictionary<string, object>>(GetTableInfo);
        }

        private IDictionary<string, object> GetTableInfo(string dataBaseName, string tableName)
        {
            var table = this.GetTable(dataBaseName, tableName);

            return table.Dispatcher.Invoke(() =>
            {
                var tableInfo = table.TableInfo;
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
    }
}
