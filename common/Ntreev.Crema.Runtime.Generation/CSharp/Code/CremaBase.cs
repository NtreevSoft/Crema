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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using Ntreev.Crema.Reader;

namespace Ntreev.Crema.Code
{
    public class CremaErrorEventArgs : EventArgs
    {
        private readonly Exception e;

        public CremaErrorEventArgs(Exception e)
        {
            this.e = e;
        }

        public Exception Exception
        {
            get { return this.e; }
        }

        public bool Handled { get; set; }
    }

    public abstract class CremaData
    {
        protected CremaData()
        {

        }

        public static event EventHandler<CremaErrorEventArgs> ErrorOccured;

        public static bool InvokeErrorOccuredEvent(object sender, Exception e)
        {
            if (ErrorOccured != null)
            {
                var args = new CremaErrorEventArgs(e);
                ErrorOccured(sender, args);
                return args.Handled;
            }
            return false;
        }
    }

    public abstract class CremaRow
    {
        private const string __RelationID__ = "__RelationID__";
        private const string __ParentID__ = "__ParentID__";

        private string relationID;
        private string parentID;
        private string key;
        private CremaRow parentInternal;

        protected CremaRow(IColumn[] columns, IRow row)
        {
            this.key = CremaUtility.GenerateHashCode(row, columns);

            for (var i = 0; i < row.Table.Columns.Count; i++)
            {
                var item = row.Table.Columns[i];
                if (item.Name == __RelationID__)
                {
                    this.relationID = row.GetValue(item.Name).ToString();
                }
                else if (item.Name == __ParentID__)
                {
                    this.parentID = row.GetValue(item.Name).ToString();
                }
            }
        }

        public string GetRelationID() { return this.relationID; }

        public string GetParentID() { return this.parentID; }

        public string GetCremaHashCode()
        {
            return this.key;
        }

        protected static void SetParent<T, U>(T parent, U[] childs)
            where T : CremaRow
            where U : CremaRow
        {
            foreach (var item in childs)
            {
                item.parentInternal = parent;
            }
        }

        protected CremaRow ParentInternal
        {
            get { return this.parentInternal; }
        }
    }

    public abstract class CremaTable<T> where T : CremaRow
    {
        private static readonly T[] emptyRows = new T[] { };
        private static readonly IColumn[] emptyColumns = new IColumn[0];

        private T[] rows;
        private IColumn[] columns;
        private Dictionary<string, T> keyToRow;
        private string name;
        private string tableName;

        protected CremaTable()
        {

        }

        protected CremaTable(ITable table)
        {
            this.name = table.Name;
            this.tableName = this.GetTableName(table.Name);
            this.keyToRow = new Dictionary<string, T>(table.Rows.Count);
            this.columns = table.Columns.ToArray();
            List<T> rows = new List<T>(table.Rows.Count);
            foreach (var item in table.Rows)
            {
                T row = this.CreateRowInstance(item, this);
                this.keyToRow.Add(row.GetCremaHashCode(), row);
                rows.Add(row);
            }
            this.rows = rows.ToArray();
        }

        protected CremaTable(string name, T[] rows)
        {
            this.name = name;
            this.tableName = this.GetTableName(name);

            if (rows.Length == 0)
                return;

            this.keyToRow = new Dictionary<string, T>(rows.Length);
            for (int i = 0; i < rows.Length; i++)
            {
                var item = rows[i];
                this.keyToRow.Add(item.GetCremaHashCode(), item);
            }
            this.rows = rows;
        }

        public T[] Rows
        {
            get
            {
                if (this.rows == null)
                    return emptyRows;
                return this.rows;
            }
        }

        public IColumn[] Columns
        {
            get
            {
                if (this.columns == null)
                    return emptyColumns;

                return this.columns;
            }
        }

        /// <summary>
        /// 테이블의 이름을 나타냅니다. 자식 테이블의 이름이 a.b. 일 경우 a.b를 반환합니다.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 부모 이름을 제외한 테이블의 이름을 나타냅니다. 만약 자식 테이블의 이름이 a.b 일 경우 b를 반환합니다.
        /// </summary>
        public string TableName
        {
            get { return this.tableName; }
        }

        protected abstract T CreateRowInstance(IRow row, object table);

        protected T FindRow(string key)
        {
            if (this.keyToRow == null)
                return null;

            if (this.keyToRow.ContainsKey(key))
                return this.keyToRow[key];

            return null;
        }

        protected void SetRelations<U>(string childName, U[] childs, Action<T, string, U[]> setChildsAction) where U : CremaRow
        {
            Dictionary<string, T> relationToRow = new Dictionary<string, T>(this.Rows.Length);

            for (int i = 0; i < this.Rows.Length; i++)
            {
                T item = this.Rows[i];
                relationToRow.Add(item.GetRelationID(), item);
            }

            Dictionary<T, List<U>> rowToChilds = new Dictionary<T, List<U>>(this.Rows.Length);

            for (int i = 0; i < childs.Length; i++)
            {
                U item = childs[i];
                if (relationToRow.ContainsKey(item.GetParentID()) == false)
                    continue;

                var parent = relationToRow[item.GetParentID()];

                if (rowToChilds.ContainsKey(parent) == false)
                    rowToChilds.Add(parent, new List<U>());

                rowToChilds[parent].Add(item);
            }

            foreach (var item in rowToChilds)
            {
                setChildsAction(item.Key, childName, item.Value.ToArray());
            }
        }

        private string GetTableName(string name)
        {
            var value = name.Split('.');
            if (value.Length == 1)
                return name;
            return value[1];
        }
    }

    public static class CremaUtility
    {
        public static string GenerateHashCode(params string[] keys)
        {
            return string.Join(",", keys);
        }

        public static string GenerateHashCode(IRow row, params IColumn[] columns)
        {
            return string.Join(",", columns.Where(column => column.IsKey).Select(column => row[column.Name]));
        }
    }
}
