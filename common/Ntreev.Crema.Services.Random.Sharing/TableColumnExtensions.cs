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
using Ntreev.Crema.Services;
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Services.Random
{
    public static class TableColumnExtensions
    {
        public static void InitializeRandom(this ITableColumn tableColumn, Authentication authentication)
        {
            var template = tableColumn.Template;
            var table = tableColumn.Template.Table;

            if (RandomUtility.Within(75) == true)
            {
                tableColumn.SetDataType(authentication, CremaDataTypeUtility.GetBaseTypeNames().Random(item => item != typeof(bool).GetTypeName()));
            }
            else 
            {
                tableColumn.SetDataType(authentication, template.SelectableTypes.Random());
            }

            if (template.Count == 0)
            {
                tableColumn.SetIsKey(authentication, true);
            }
            else if (RandomUtility.Within(10) && tableColumn.DataType != typeof(bool).GetTypeName())
            {
                tableColumn.SetIsKey(authentication, true);
                tableColumn.SetIsUnique(authentication, RandomUtility.Within(75));
            }

            if (RandomUtility.Within(25) && tableColumn.DataType != typeof(bool).GetTypeName())
            {
                var unique = RandomUtility.Within(75);
                if (unique != false || template.PrimaryKey.Count() != 1)
                {
                    tableColumn.SetIsUnique(authentication, unique);
                }
            }

            if (RandomUtility.Within(25) == true)
            {
                tableColumn.SetComment(authentication, RandomUtility.NextString());
            }

            if (RandomUtility.Within(25) == true)
            {
                tableColumn.SetDefaultValue(authentication, tableColumn.GetRandomString());
            }

            if (CremaDataTypeUtility.CanUseAutoIncrement(tableColumn.DataType) == true && tableColumn.DefaultValue == null)
            {
                tableColumn.SetAutoIncrement(authentication, RandomUtility.NextBoolean());
            }

            if (RandomUtility.Within(5) == true)
            {
                tableColumn.SetIsReadOnly(authentication, true);
            }
        }

        public static void ModifyRandomValue(this ITableColumn tableColumn, Authentication authentication)
        {
            if (RandomUtility.Within(75) == true)
            {
                SetRandomName(tableColumn, authentication);
            }
            else if (RandomUtility.Within(75) == true)
            {
                SetRandomValue(tableColumn, authentication);
            }
            else
            {
                SetRandomComment(tableColumn, authentication);
            }
        }

        public static void ExecuteRandomTask(this ITableColumn tableColumn, Authentication authentication)
        {
            if (RandomUtility.Within(75) == true)
            {
                SetRandomName(tableColumn, authentication);
            }
            else if (RandomUtility.Within(75) == true)
            {
                SetRandomValue(tableColumn, authentication);
            }
            else
            {
                SetRandomComment(tableColumn, authentication);
            }
        }

        public static void SetRandomName(this ITableColumn tableColumn, Authentication authentication)
        {
            var newName = RandomUtility.NextIdentifier();
            tableColumn.SetName(authentication, newName);
        }

        public static void SetRandomValue(this ITableColumn tableColumn, Authentication authentication)
        {
            //if (tableColumn.Template.IsFlag == true)
            //{
            //    tableColumn.SetValue(authentication, RandomUtility.NextBit());
            //}
            //else
            //{
            //    tableColumn.SetValue(authentication, RandomUtility.NextLong(long.MaxValue));
            //}
        }

        public static void SetRandomComment(this ITableColumn tableColumn, Authentication authentication)
        {
            if (RandomUtility.Within(50) == true)
            {
                tableColumn.SetComment(authentication, RandomUtility.NextString());
            }
            else
            {
                tableColumn.SetComment(authentication, string.Empty);
            }
        }

        public static string GetRandomString(this ITableColumn tableColumn)
        {
            if (tableColumn.DefaultValue != null && RandomUtility.Next(3) == 0)
            {
                return null;
            }
            else if (tableColumn.AllowNull == true && RandomUtility.Next(4) == 0)
            {
                return null;
            }
            else
            {
                var template = tableColumn.Template;
                var dataType = tableColumn.DataType;
                if (CremaDataTypeUtility.IsBaseType(dataType) == false)
                {
                    var type = template.GetType(dataType);
                    return type.GetRandomString();
                }
                else
                {
                    var value = RandomUtility.Next(CremaDataTypeUtility.GetType(dataType));
                    return CremaConvert.ChangeType(value, typeof(string)) as string;
                }
            }
        }
    }
}
