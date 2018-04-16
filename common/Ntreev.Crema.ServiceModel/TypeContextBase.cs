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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.ServiceModel.Properties;
using Ntreev.Crema.Data;
using Ntreev.Library.Linq;
using System.Text.RegularExpressions;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class TypeContextBase<_I, _C, _IC, _CC, _CT> : ItemContext<_I, _C, _IC, _CC, _CT>
        where _I : TypeBase<_I, _C, _IC, _CC, _CT>
        where _C : TypeCategoryBase<_I, _C, _IC, _CC, _CT>, new()
        where _IC : ItemContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CC : CategoryContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CT : TypeContextBase<_I, _C, _IC, _CC, _CT>
    {
        public TypeContextBase()
        {
            this.ItemRenamed += TypeContextBase_ItemRenamed;
            this.ItemMoved += TypeContextBase_ItemMoved;
            this.ItemDeleted += TypeContextBase_ItemDeleted;

            this.CategoryRenamed += TypeContextBase_CategoryRenamed;
            this.CategoryMoved += TypeContextBase_CategoryMoved;
            this.CategoryDeleted += TypeContextBase_CategoryDeleted;
        }

        protected abstract IEnumerable<ITableInfoProvider> GetTables();

        private void TypeContextBase_ItemRenamed(object sender, ItemRenamedEventArgs<_I> e)
        {
            foreach (var item in this.GetTables())
            {
                var tableInfo = item.TableInfo;
                this.RenameType(ref tableInfo, e.OldPath, e.Item.Path);
                item.TableInfo = tableInfo;
            }
        }

        private void TypeContextBase_ItemMoved(object sender, ItemMovedEventArgs<_I> e)
        {
            foreach (var item in this.GetTables())
            {
                var tableInfo = item.TableInfo;
                this.MoveType(ref tableInfo, e.OldPath, e.Item.Path);
                item.TableInfo = tableInfo;
            }
        }

        private void TypeContextBase_ItemDeleted(object sender, ItemDeletedEventArgs<_I> e)
        {
            foreach (var item in this.GetTables())
            {
                var tableInfo = item.TableInfo;
                this.DeleteType(ref tableInfo, e.ItemPath);
                item.TableInfo = tableInfo;
            }
        }

        private void TypeContextBase_CategoryRenamed(object sender, CategoryRenamedEventArgs<_C> e)
        {
            foreach (var item in this.GetTables())
            {
                var tableInfo = item.TableInfo;
                this.RenameTypeCategory(ref tableInfo, e.OldPath, e.Category.Path);
                item.TableInfo = tableInfo;
            }
        }

        private void TypeContextBase_CategoryMoved(object sender, CategoryMovedEventArgs<_C> e)
        {
            foreach (var item in this.GetTables())
            {
                var tableInfo = item.TableInfo;
                this.MoveTypeCategory(ref tableInfo, e.OldPath, e.Category.Path);
                item.TableInfo = tableInfo;
            }
        }

        private void TypeContextBase_CategoryDeleted(object sender, CategoryDeletedEventArgs<_C> e)
        {
            foreach (var item in this.GetTables())
            {
                var tableInfo = item.TableInfo;
                this.DeleteTypeCategory(ref tableInfo, e.CategoryPath);
                item.TableInfo = tableInfo;
            }
        }

        private void RenameType(ref TableInfo tableInfo, string typePath, string newTypePath)
        {
            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                this.RenameType(ref tableInfo.Columns[i], typePath, newTypePath);
            }
        }

        private void RenameType(ref ColumnInfo columnInfo, string typePath, string newTypePath)
        {
            if (columnInfo.DataType == typePath)
            {
                columnInfo.DataType = newTypePath;
            }
        }

        private void MoveType(ref TableInfo tableInfo, string typePath, string newTypePath)
        {
            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                this.MoveType(ref tableInfo.Columns[i], typePath, newTypePath);
            }
        }

        private void MoveType(ref ColumnInfo columnInfo, string typePath, string newTypePath)
        {
            if (columnInfo.DataType == typePath)
            {
                columnInfo.DataType = newTypePath;
            }
        }

        private void DeleteType(ref TableInfo tableInfo, string typePath)
        {
            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                this.DeleteType(ref tableInfo.Columns[i], typePath);
            }
        }

        private void DeleteType(ref ColumnInfo columnInfo, string typePath)
        {
            if (columnInfo.DataType == typePath)
            {
                columnInfo.DataType = typeof(string).GetTypeName();
            }
        }

        private void RenameTypeCategory(ref TableInfo tableInfo, string categoryPath, string newCategoryPath)
        {
            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                this.RenameTypeCategory(ref tableInfo.Columns[i], categoryPath, newCategoryPath);
            }
        }

        private void RenameTypeCategory(ref ColumnInfo columnInfo, string categoryPath, string newCategoryPath)
        {
            var dataType = columnInfo.DataType;
            if (dataType.StartsWith(categoryPath) == true)
            {
                columnInfo.DataType = Regex.Replace(dataType, "^" + categoryPath, newCategoryPath);
            }
        }

        private void MoveTypeCategory(ref TableInfo tableInfo, string categoryPath, string newCategoryPath)
        {
            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                this.MoveTypeCategory(ref tableInfo.Columns[i], categoryPath, newCategoryPath);
            }
        }

        private void MoveTypeCategory(ref ColumnInfo columnInfo, string categoryPath, string newCategoryPath)
        {
            var dataType = columnInfo.DataType;
            if (dataType.StartsWith(categoryPath) == true)
            {
                columnInfo.DataType = Regex.Replace(dataType, "^" + categoryPath, newCategoryPath);
            }
        }

        private void DeleteTypeCategory(ref TableInfo tableInfo, string categoryPath)
        {
            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                this.DeleteTypeCategory(ref tableInfo.Columns[i], categoryPath);
            }
        }

        private void DeleteTypeCategory(ref ColumnInfo columnInfo, string categoryPath)
        {
            if (columnInfo.DataType.StartsWith(categoryPath) == true)
            {
                columnInfo.DataType = typeof(string).GetTypeName();
            }
        }
    }
}
