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

namespace Ntreev.Crema.Runtime.Generation.CSharp
{
    static class ColumnInfoExtensions
    {
        public static string TypeNamespace { get; set; }

        public static string GetFieldName(this ColumnInfo columnInfo)
        {
            return "field_" + columnInfo.Name;
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
            return Utility.GenerateCodeType(TypeNamespace, itemName.Name);
        }

        public static bool IsCustomType(this ColumnInfo columnInfo)
        {
            return CremaDataTypeUtility.IsBaseType(columnInfo.DataType) == false;
        }

        public static CodeExpression GetFieldExpression(this ColumnInfo columnInfo)
        {
            return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), columnInfo.GetFieldName());
        }

        public static CodeExpression GetGetValueMethodExpression(this ColumnInfo columnInfo)
        {
            var row = new CodeVariableReferenceExpression("row");
            var getValue = new CodeMethodReferenceExpression(row, GetMethodName(columnInfo));
            var columnName = new CodePrimitiveExpression(columnInfo.Name);
            var getValueInvoke = new CodeMethodInvokeExpression(getValue, columnName);

            if (columnInfo.IsCustomType() == false)
                return getValueInvoke;

            return new CodeCastExpression(columnInfo.GetCodeType(), getValueInvoke);
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

                if (columnInfo.IsCustomType() == false)
                    return getValueInvoke;

                return new CodeCastExpression(columnInfo.GetCodeType(), getValueInvoke);
            }
        }

        public static CodeExpression GetHasValueMethodExpression(this ColumnInfo columnInfo)
        {
            var rowVarRefExp = new CodeVariableReferenceExpression("row");
            var columnNameExp = new CodePrimitiveExpression(columnInfo.Name);
            return new CodeMethodInvokeExpression(rowVarRefExp, "HasValue", columnNameExp);
        }

        public static CodeExpression GetHasValueMethodExpression(this ColumnInfo columnInfo, int index, bool isDevmode)
        {
            if (isDevmode == true)
            {
                return GetHasValueMethodExpression(columnInfo);
            }
            else
            {
                var rowVarRefExp = new CodeVariableReferenceExpression("row");
                var columnNameExp = new CodePrimitiveExpression(index);
                return new CodeMethodInvokeExpression(rowVarRefExp, "HasValue", columnNameExp);
            }
        }

        public static string GetMethodName(this ColumnInfo columnInfo)
        {
            if (columnInfo.DataType == CremaDataTypeUtility.booleanType)
                return "ToBoolean";
            else if (columnInfo.DataType == CremaDataTypeUtility.stringType)
                return "ToString";
            else if (columnInfo.DataType == "float" || columnInfo.DataType == CremaDataTypeUtility.floatType)
                return "ToSingle";
            else if (columnInfo.DataType == CremaDataTypeUtility.doubleType)
                return "ToDouble";
            else if (columnInfo.DataType == "byte" || columnInfo.DataType == CremaDataTypeUtility.int8Type)
                return "ToInt8";
            else if (columnInfo.DataType == "unsignedByte" || columnInfo.DataType == CremaDataTypeUtility.uint8Type)
                return "ToUInt8";
            else if (columnInfo.DataType == "short" || columnInfo.DataType == CremaDataTypeUtility.int16Type)
                return "ToInt16";
            else if (columnInfo.DataType == "unsignedShort" || columnInfo.DataType == CremaDataTypeUtility.uint16Type)
                return "ToUInt16";
            else if (columnInfo.DataType == "int" || columnInfo.DataType == CremaDataTypeUtility.int32Type)
                return "ToInt32";
            else if (columnInfo.DataType == "unsignedInt" || columnInfo.DataType == CremaDataTypeUtility.uint32Type)
                return "ToUInt32";
            else if (columnInfo.DataType == "long" || columnInfo.DataType == CremaDataTypeUtility.int64Type)
                return "ToInt64";
            else if (columnInfo.DataType == "unsignedLong" || columnInfo.DataType == CremaDataTypeUtility.uint64Type)
                return "ToUInt64";
            else if (columnInfo.DataType == "dateTime" || columnInfo.DataType == CremaDataTypeUtility.datetimeType)
                return "ToDateTime";
            else if (columnInfo.DataType == CremaDataTypeUtility.durationType)
                return "ToDuration";
            else if (columnInfo.DataType == CremaDataTypeUtility.guidType)
                return "ToGuid";

            return "ToInt32";
        }
    }
}
