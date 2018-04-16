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
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Runtime.Generation.TypeScript
{
    static class ColumnInfoExtensions
    {
        public static string TypeNamespace { get; set; }

        public static string GetFieldName(this ColumnInfo columnInfo)
        {
            return "field" + columnInfo.Name;
        }

        public static string GetPropertyName(this ColumnInfo columnInfo)
        {
            return columnInfo.Name;
        }

        public static CodeTypeReference GetCodeType(this ColumnInfo columnInfo)
        {
            if (CremaDataTypeUtility.IsBaseType(columnInfo.DataType) == true)
            {
                return new CodeTypeReference(CremaDataTypeUtility.GetType(columnInfo.DataType));
            }

            var itemName = new ItemName(columnInfo.DataType);
            if (string.IsNullOrEmpty(TypeNamespace) == true)
                return new CodeTypeReference(itemName.Name);
            
            return new CodeTypeReference(TypeNamespace + "." + itemName.Name);
        }

        public static CodeExpression GetFieldExpression(this ColumnInfo columnInfo)
        {
            return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), columnInfo.GetFieldName());
        }

        public static CodeExpression GetGetValueMethodExpression(this ColumnInfo columnInfo)
        {
            var row = new CodeVariableReferenceExpression("row");
            var getValue = new CodeMethodReferenceExpression(row, GetMethodName(columnInfo) + "ByString");
            var columnName = new CodePrimitiveExpression(columnInfo.Name);
            var getValueInvoke = new CodeMethodInvokeExpression(getValue, columnName);

            return getValueInvoke;
        }

        public static CodeExpression GetGetValueMethodExpression(this ColumnInfo columnInfo, int index, bool isDevmode)
        {
            if (isDevmode == true)
            {
                return GetGetValueMethodExpression(columnInfo);
            }
            else
            {
                var row = new CodeVariableReferenceExpression("row");
                var getValue = new CodeMethodReferenceExpression(row, GetMethodName(columnInfo));
                var indexVar = new CodePrimitiveExpression(index);
                var getValueInvoke = new CodeMethodInvokeExpression(getValue, indexVar);

                return getValueInvoke;
            }
        }

        public static CodeExpression GetHasValueMethodExpression(this ColumnInfo columnInfo)
        {
            var row = new CodeVariableReferenceExpression("row");
            var columnName = new CodePrimitiveExpression(columnInfo.Name);
            return new CodeMethodInvokeExpression(row, "hasValueByString", columnName);
        }

        public static CodeExpression GetHasValueMethodExpression(this ColumnInfo columnInfo, int index, bool isDevmode)
        {
            if (isDevmode == true)
            {
                return GetHasValueMethodExpression(columnInfo);
            }
            else
            {
                var row = new CodeVariableReferenceExpression("row");
                var indexVar = new CodePrimitiveExpression(index);
                return new CodeMethodInvokeExpression(row, "hasValue", indexVar);
            }
        }

        public static string GetMethodName(this ColumnInfo columnInfo)
        {
            if (columnInfo.DataType == "boolean")
                return "toBoolean";
            else if (columnInfo.DataType == "string")
                return "toString";
            else if (columnInfo.DataType == "float")
                return "toSingle";
            else if (columnInfo.DataType == "float")
                return "toDouble";
            else if (columnInfo.DataType == "byte")
                return "toInt8";
            else if (columnInfo.DataType == "unsignedByte")
                return "toUInt8";
            else if (columnInfo.DataType == "short")
                return "toInt16";
            else if (columnInfo.DataType == "unsignedShort")
                return "toUInt16";
            else if (columnInfo.DataType == "int")
                return "toInt32";
            else if (columnInfo.DataType == "unsignedInt")
                return "toUInt32";
            else if (columnInfo.DataType == "long")
                return "toInt64";
            else if (columnInfo.DataType == "unsignedLong")
                return "toUInt64";
            else if (columnInfo.DataType == "dateTime")
                return "toDateTime";
            else if (columnInfo.DataType == "duration")
                return "toDuration";
            else if (columnInfo.DataType == "dictionary")
                return "toString";
            else if (columnInfo.DataType == "table")
                return "toString";

            return "toInt32";
        }
    }
}
