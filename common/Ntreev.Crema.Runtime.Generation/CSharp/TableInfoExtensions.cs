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
using Ntreev.Library;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.CSharp
{
    static class TableInfoExtensions
    {
        private const string seperator = "";
        private const string tablePrefix = "";
        private const string rowPostfix = "Row";

        public static string GetFieldName(this TableInfo tableInfo)
        {
            return "table" + tableInfo.TableName;
        }

        public static string GetVariableName(this TableInfo tableInfo)
        {
            return Regex.Replace(tableInfo.GetPropertyName(), "^[a-zA-Z]", i => i.Value.ToLower());
        }

        public static string GetPropertyName(this TableInfo tableInfo)
        {
            return tableInfo.TableName;
        }

        public static string GetClassName(this TableInfo tableInfo)
        {
            string tableName = tableInfo.TemplatedParent == string.Empty ? tableInfo.TableName : tableInfo.GetTemplatedParentTableName();
            if (string.IsNullOrEmpty(tableInfo.ParentName) == false)
                return string.Join(seperator, tableInfo.ParentName, tableName, tablePrefix);
            return string.Join(seperator, tableName, tablePrefix);
        }

        public static string GetParentClassName(this TableInfo tableInfo)
        {
            if (string.IsNullOrEmpty(tableInfo.ParentName) == false)
                return string.Join(seperator, tableInfo.ParentName, tablePrefix);
            return string.Empty;
        }

        public static string GetRowClassName(this TableInfo tableInfo)
        {
            string tableName = tableInfo.TemplatedParent == string.Empty ? tableInfo.TableName : tableInfo.GetTemplatedParentTableName();
            if (string.IsNullOrEmpty(tableInfo.ParentName) == false)
                return string.Join(seperator, tableInfo.ParentName, tableName, rowPostfix);
            return string.Join(seperator, tableName, rowPostfix);
        }

        public static string GetParentRowClassName(this TableInfo tableInfo)
        {
            if (tableInfo.ParentName != string.Empty)
                return string.Join(seperator, tableInfo.ParentName, rowPostfix);
            return string.Empty;
        }

        public static string GetTemplatedParentTableName(this TableInfo tableInfo)
        {
            if (tableInfo.TemplatedParent == string.Empty)
                throw new CremaDataException();

            if (tableInfo.TemplatedParent.Contains('.') == true)
            {
                var index = tableInfo.TemplatedParent.LastIndexOf(".");
                return tableInfo.TemplatedParent.Substring(index + 1, tableInfo.TemplatedParent.Length - index - 1);
                //return StringUtility.Split(tableInfo.TemplatedParent, '.')[1];
            }
            return tableInfo.TemplatedParent;
        }

        public static CodeTypeReference GetCodeType(this TableInfo tableInfo)
        {
            return new CodeTypeReference(tableInfo.GetClassName());
        }

        public static CodeTypeReference GetRowCodeType(this TableInfo tableInfo)
        {
            return new CodeTypeReference(tableInfo.GetRowClassName());
        }

        public static CodeTypeReferenceExpression GetRowCodeTypeExpression(this TableInfo tableInfo)
        {
            return new CodeTypeReferenceExpression(tableInfo.GetRowCodeType());
        }

        public static CodeTypeReference GetParentRowCodeType(this TableInfo tableInfo)
        {
            return new CodeTypeReference(tableInfo.GetParentRowClassName());
        }

        public static CodeExpression GetFieldExpression(this TableInfo tableInfo)
        {
            return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), tableInfo.GetFieldName());
        }

        public static CodeExpression GetFieldExpression(this TableInfo tableInfo, CodeVariableReferenceExpression varRefExp)
        {
            return new CodeFieldReferenceExpression(varRefExp, tableInfo.GetFieldName());
        }
    }
}
