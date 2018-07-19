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

using Ntreev.Crema.Data.Properties;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;

namespace Ntreev.Crema.Data
{
    class InternalDataSet : InternalSetBase, INotifyPropertyChanged
    {
        private int loadingCount;

        public InternalDataSet(CremaDataSet target, string dataSetName)
            : base(dataSetName)
        {
            base.Target = target;
            this.Namespace = CremaSchema.BaseNamespace;
        }

        public static string GenerateHashValue(params TypeInfo[] types)
        {
            var args = types.Select(item => item.HashValue).ToArray();
            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, args);
            }
        }

        public static string GenerateHashValue(params TableInfo[] tables)
        {
            var args = tables.Select(item => item.HashValue).ToArray();
            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, args);
            }
        }

        public InternalDataTable AddTable()
        {
            var name = NameUtility.GenerateNewName("Table", GetTableNames());
            return this.AddTable(name);

            IEnumerable<string> GetTableNames()
            {
                foreach (var item in this.Tables)
                {
                    if (item is InternalDataTable tableItem)
                        yield return tableItem.Name;
                }
            }
        }

        public InternalDataTable AddTable(string name)
        {
            return this.AddTable(name, PathUtility.Separator);
        }

        public InternalDataTable AddTable(string name, string categoryPath)
        {
            var dataTable = new InternalDataTable(name, categoryPath);
            var signatureDate = this.Sign();
            this.Tables.Add(dataTable);
            dataTable.InternalCreationInfo = signatureDate;
            dataTable.InternalModificationInfo = signatureDate;
            return dataTable;
        }

        public InternalDataTable AddTable(TableInfo tableInfo)
        {
            this.ValidateAddTable(tableInfo);
            var dataTable = new InternalDataTable(tableInfo.TableName, tableInfo.CategoryPath)
            {
                InternalTableID = tableInfo.ID,
                InternalTags = tableInfo.Tags,
                InternalComment = tableInfo.Comment
            };

            this.Tables.Add(dataTable);

            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                dataTable.AddColumn(tableInfo.Columns[i]);
            }

            dataTable.InternalCreationInfo = tableInfo.CreationInfo;
            dataTable.InternalModificationInfo = tableInfo.ModificationInfo;
            dataTable.InternalContentsInfo = tableInfo.ContentsInfo;
            dataTable.AcceptChanges();
            return dataTable;
        }

        public InternalDataType AddType(TypeInfo typeInfo)
        {
            this.ValidateAddType(typeInfo);
            var dataType = new InternalDataType(typeInfo.Name, typeInfo.CategoryPath)
            {
                InternalTypeID = typeInfo.ID,
                InternalTags = typeInfo.Tags,
                InternalComment = typeInfo.Comment,
                InternalIsFlag = typeInfo.IsFlag,
            };

            dataType.OmitSignatureDate = true;
            for (var i = 0; i < typeInfo.Members.Length; i++)
            {
                dataType.AddMember(typeInfo.Members[i]);
            }
            dataType.OmitSignatureDate = false;

            this.Tables.Add(dataType);

            dataType.InternalCreationInfo = typeInfo.CreationInfo;
            dataType.InternalModificationInfo = typeInfo.ModificationInfo;
            dataType.AcceptChanges();
            return dataType;
        }

        public void AddTable(InternalDataTable dataTable)
        {
            this.ValidateAddTable(dataTable);
            this.Tables.Add(dataTable);
            foreach (var item in dataTable.ChildItems)
            {
                this.Tables.Add(item);
            }
        }

        public void RemoveTable(InternalDataTable dataTable)
        {
            this.ValidateRemoveTable(dataTable);
            this.Sign();

            if (dataTable.ParentName != string.Empty)
            {
                var derivedItems = dataTable.DerivedItems.ToArray();
                foreach (var item in derivedItems)
                {
                    this.Tables.Remove(item);
                }
            }

            foreach (var item in dataTable.ChildItems)
            {
                this.Tables.Remove(item);
            }
            this.Tables.Remove(dataTable);
        }

        public bool CanRemoveTable(InternalDataTable dataTable)
        {
            if (dataTable == null)
                return false;
            if (dataTable.DataSet != this)
                return false;
            if (dataTable.ParentName != string.Empty && dataTable.TemplatedParentName != string.Empty)
                return false;
            return true;
        }

        public void BeginLoad()
        {
            lock (CremaSchema.lockobj)
            {
                if (this.loadingCount == 0)
                {
                    foreach (var item in this.Tables)
                    {
                        if (item is InternalTableBase table)
                        {
                            table.BeginLoadData();
                        }
                    }
                    this.EnforceConstraints = false;
                }
                this.loadingCount++;
            }
        }

        public void EndLoad()
        {
            if (this.loadingCount == 0)
                throw new InvalidOperationException(nameof(EndLoad));

            lock (CremaSchema.lockobj)
            {
                this.loadingCount--;
                if (this.loadingCount == 0)
                {
                    foreach (var item in this.Tables)
                    {
                        if (item is InternalTableBase table && table.IsLoading == true)
                        {
                            table.EndLoadData();
                        }
                    }
                    this.EnforceConstraints = true;
                }
            }
        }

        public static explicit operator CremaDataSet(InternalDataSet dataSet)
        {
            if (dataSet == null)
                return null;
            return dataSet.Target as CremaDataSet;
        }

        public static explicit operator InternalDataSet(CremaDataSet dataSet)
        {
            if (dataSet == null)
                return null;
            return dataSet.InternalObject;
        }

        public new CremaDataSet Target => base.Target as CremaDataSet;

        public new string Namespace
        {
            get => base.Namespace;
            set
            {
                base.Namespace = value;

                if (base.Namespace == CremaSchemaObsolete.BaseNamespaceObsolete)
                {
                    this.TableNamespace = UriUtility.Combine(base.Namespace, CremaSchemaObsolete.TableDirectoryObsolete);
                    this.TypeNamespace = UriUtility.Combine(base.Namespace, CremaSchemaObsolete.TypeDirectoryObsolete);
                }
                else
                {
                    this.TableNamespace = UriUtility.Combine(base.Namespace, CremaSchema.TableDirectory);
                    this.TypeNamespace = UriUtility.Combine(base.Namespace, CremaSchema.TypeDirectory);
                }

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Namespace)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.TableNamespace)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.TypeNamespace)));
            }
        }

        public string TableNamespace { get; private set; }

        public string TypeNamespace { get; private set; }

        public bool IsLoading => this.loadingCount > 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public string GetTableCategoryPath(string tableNamespace)
        {
            return GetTableCategoryPath(this, tableNamespace);
        }

        public static string GetTableCategoryPath(InternalDataSet dataSet, string tableNamespace)
        {
            var baseNamespace = dataSet != null ? dataSet.TableNamespace : CremaSchema.TableNamespace;
            return GetTableCategoryPath(baseNamespace, tableNamespace);
        }

        public static string GetTableCategoryPath(string baseNamespace, string tableNamespace)
        {
            if (tableNamespace.StartsWith(baseNamespace) == false)
                throw new ArgumentException();

            var path = tableNamespace.Substring(baseNamespace.Length);
            var itemName = new ItemName(path);
            return itemName.CategoryPath;
        }

        public string GetTableName(string tableNamespace)
        {
            return GetTableName(this, tableNamespace);
        }

        public static string GetTableName(InternalDataSet dataSet, string tableNamespace)
        {
            var baseNamespace = dataSet != null ? dataSet.TableNamespace : CremaSchema.TableNamespace;
            return GetTableName(baseNamespace, tableNamespace);
        }

        public static string GetTableName(string baseNamespace, string tableNamespace)
        {
            if (tableNamespace.StartsWith(baseNamespace) == false)
                throw new ArgumentException();

            var path = tableNamespace.Substring(baseNamespace.Length);
            var itemName = new ItemName(path);
            return itemName.Name;
        }

        public string GetTypeCategoryPath(string typeNamespace)
        {
            return GetTypeCategoryPath(this, typeNamespace);
        }

        public static string GetTypeCategoryPath(InternalDataSet dataSet, string typeNamespace)
        {
            var baseNamespace = dataSet != null ? dataSet.TypeNamespace : CremaSchema.TypeNamespace;
            return GetTypeCategoryPath(baseNamespace, typeNamespace);
        }

        public static string GetTypeCategoryPath(string baseNamespace, string typeNamespace)
        {
            if (typeNamespace.StartsWith(baseNamespace) == false)
                throw new ArgumentException();

            var path = typeNamespace.Substring(baseNamespace.Length);
            var itemName = new ItemName(path);
            return itemName.CategoryPath;
        }

        public string GetTypeName(string typeNamespace)
        {
            return GetTypeName(this, typeNamespace);
        }

        public static string GetTypeName(InternalDataSet dataSet, string typeNamespace)
        {
            var baseNamespace = dataSet != null ? dataSet.TypeNamespace : CremaSchema.TypeNamespace;
            return GetTypeName(baseNamespace, typeNamespace);
        }

        public static string GetTypeName(string baseNamespace, string typeNamespace)
        {
            if (typeNamespace.StartsWith(baseNamespace) == false)
                throw new ArgumentException();

            var path = typeNamespace.Substring(baseNamespace.Length);
            var itemName = new ItemName(path);
            return itemName.Name;
        }

        public static bool ContainsType(InternalDataSet dataSet, string name, string categoryPath)
        {
            if (dataSet == null)
                return false;
            var itemNamespace = UriUtility.Combine(dataSet.TypeNamespace + categoryPath, name);
            foreach (var item in dataSet.Tables)
            {
                if (item is InternalDataType dataType)
                {
                    if (dataType.Name == name && dataType.Namespace == itemNamespace)
                        return true;
                }
            }
            return false;
        }

        public static bool ContainsType(InternalDataSet dataSet, string typeName)
        {
            if (dataSet == null)
                return false;
            foreach (var item in dataSet.Tables)
            {
                if (item is InternalDataType dataType)
                {
                    if (dataType.LocalName == typeName)
                        return true;
                }
            }
            return false;
        }

        public static InternalDataType GetType(InternalDataSet dataSet, string name, string categoryPath)
        {
            var itemNamespace = UriUtility.Combine(dataSet.TypeNamespace + categoryPath, name);
            foreach (var item in dataSet.Tables)
            {
                if (item is InternalDataType dataType)
                {
                    if (dataType.Name == name && dataType.Namespace == itemNamespace)
                        return dataType;
                }
            }
            return null;
        }

        public static InternalDataType GetType(InternalDataSet dataSet, string typeName)
        {
            foreach (var item in dataSet.Tables)
            {
                if (item is InternalDataType dataType)
                {
                    if (dataType.LocalName == typeName)
                        return dataType;
                }
            }
            return null;
        }

        public static void ValidateSetDataTypeName(InternalDataSet dataSet, string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value is string == false)
                throw new ArgumentException(nameof(value));

            if (dataSet == null && CremaDataTypeUtility.IsBaseType(value) == false)
            {
                if (NameValidator.VerifyItemPath(value) == false)
                    throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, value), nameof(value));
            }

            if (dataSet != null && CremaDataTypeUtility.IsBaseType(value) == false)
            {
                if (NameValidator.VerifyItemPath(value) == false)
                {
                    if (InternalDataSet.ContainsType(dataSet, value) == false)
                        throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, value), nameof(value));
                }
                else
                {
                    var itemName = new ItemName(value);
                    if (InternalDataSet.ContainsType(dataSet, itemName.Name, itemName.CategoryPath) == false)
                        throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, value), nameof(value));
                }
            }
        }

        private void ValidateAddTable(InternalDataTable dataTable)
        {
            if (dataTable.DataSet != null)
                throw new CremaDataException("이미 DataSet에 속해 있는 테이블은 추가할 수 없습니다.");
            if (dataTable.Parent != null && dataTable.Parent.DataSet != this)
                throw new CremaDataException(Resources.Exception_CannotAddChildTable);
        }

        private void ValidateAddTable(TableInfo tableInfo)
        {
            if (tableInfo.ParentName != string.Empty)
                throw new CremaDataException(Resources.Exception_CannotAddChildTable);
            if (tableInfo.TemplatedParent != string.Empty)
                throw new CremaDataException(Resources.Exception_CannotAddInheritedTable);
        }

        private void ValidateRemoveTable(InternalDataTable dataTable)
        {
            if (dataTable.DataSet != this)
                throw new CremaDataException("DataSet에 속해 있지 않은 테이블은 제거할 수 없습니다.");
            //if (dataTable.Parent != null)
            //    throw new CremaDataException(Resources.Exception_CannotRemoveChildTable);
            if (dataTable.ParentName != string.Empty && dataTable.TemplatedParentName != string.Empty)
                throw new CremaDataException("상속된 자식 테이블은 제거할 수 없습니다.");
        }

        private void ValidateAddType(TypeInfo typeInfo)
        {
            //if (tableInfo.ParentName != string.Empty)
            //    throw new CremaDataException("자식 테이블은 추가할 수 없습니다.");
            //if (tableInfo.TemplatedParent != string.Empty)
            //    throw new CremaDataException("상속받은 테이블은 추가할 수 없습니다.");
        }
    }
}
