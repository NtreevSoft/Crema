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

using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Random
{
    public static class CremaDataColumnExtensions
    {
        private const string hopeTypeKey = "HopeType";

        public static void SetHopeType(this CremaDataColumn dataColumn, object dataType)
        {
            dataColumn.ExtendedProperties[hopeTypeKey] = dataType;
        }

        public static object GetHopeType(this CremaDataColumn dataColumn)
        {
            if (dataColumn.ExtendedProperties.ContainsKey(hopeTypeKey) == false)
            {
                if (dataColumn.CremaType != null)
                    return dataColumn.CremaType;
                return dataColumn.DataType;
            }
            return dataColumn.ExtendedProperties[hopeTypeKey];
        }

        public static object GetRandomValue(this CremaDataColumn dataColumn)
        {
            if (dataColumn.DefaultValue != DBNull.Value && RandomUtility.Next(3) == 0)
            {
                return null;
            }
            else if (dataColumn.AllowDBNull == true && RandomUtility.Next(4) == 0)
            {
                return DBNull.Value;
            }
            else
            {
                var hopeType = GetHopeType(dataColumn);
                if (hopeType is CremaDataType)
                {
                    return (hopeType as CremaDataType).GetRandomValue();
                }
                else
                {
                    return RandomUtility.Next(hopeType as Type);
                }
            }
        }

        public static void InitializeRandom(this CremaDataColumn dataColumn)
        {
            var dataTable = dataColumn.Table;

            if (RandomUtility.Within(75) == true)
            {
                dataColumn.DataType = CremaDataTypeUtility.GetBaseTypes().Random();
            }
            else if (dataTable != null && dataTable.DataSet != null && dataTable.DataSet.Types.Any())
            {
                dataColumn.CremaType = dataTable.DataSet.Types.Random();
            }

            if (RandomUtility.Within(25) == true)
            {
                SetHopeType(dataColumn);
            }

            if (dataTable != null && dataTable.PrimaryKey.Any() == false)
            {
                dataColumn.IsKey = true;
            }
            else if (RandomUtility.Within(10) && dataColumn.DataType != typeof(bool))
            {
                dataColumn.IsKey = true;
                dataColumn.Unique = RandomUtility.Within(75);
            }

            if (RandomUtility.Within(25) && dataColumn.DataType != typeof(bool))
            {
                var unique = RandomUtility.Within(75);
                if (unique != false || dataTable == null || dataTable.PrimaryKey.Count() != 1)
                {
                    dataColumn.Unique = unique;
                }
            }

            if (RandomUtility.Within(25) == true)
            {
                dataColumn.Comment = RandomUtility.NextString();
            }

            if (RandomUtility.Within(25) == true)
            {
                dataColumn.DefaultValue = dataColumn.GetRandomValue();
            }

            if (CremaDataTypeUtility.CanUseAutoIncrement(dataColumn.DataType) == true && dataColumn.DefaultValue == DBNull.Value)
            {
                dataColumn.AutoIncrement = RandomUtility.NextBoolean();
            }

            if (RandomUtility.Within(5) == true)
            {
                dataColumn.ReadOnly = true;
            }
        }

        //[Obsolete]
        private static void SetHopeType(CremaDataColumn dataColumn)
        {
            //var dataTable = dataColumn.Table;

            //if (dataColumn.DataType == typeof(string))
            //{
            //    if (dataColumn.CremaType == null && dataTable.DataSet != null)
            //    {
            //        SetHopeType(dataColumn, dataTable.DataSet.Types.Random());
            //    }
            //    else
            //    {
            //        SetHopeType(dataColumn, CremaDataTypeUtility.GetBaseTypes().Random(item => item != typeof(string)));
            //    }
            //}
            //else
            //{
            //    //SetHopeType(dataColumn, CremaDataTypeUtility.GetBaseTypes().Random(item => item != dataColumn.DataType));
            //}
        }
    }
}
