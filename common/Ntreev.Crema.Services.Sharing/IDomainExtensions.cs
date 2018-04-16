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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public static class IDomainExtensions
    {
        public static object[] NewRow(this IDomain domain, Authentication authentication, string tableName, object[] fields)
        {
            var row = new DomainRowInfo()
            {
                TableName = tableName,
                Fields = fields,
            };

            return domain.NewRow(authentication, new DomainRowInfo[] { row }).First().Keys;
        }

        public static void BeginEdit(this IDomain domain, Authentication authentication, object item, string fieldName)
        {
            var location = new DomainLocationInfo()
            {
                TableName = CremaDataRowUtility.GetTableName(item),
                Keys = CremaDataRowUtility.GetKeys(item),
                ColumnName = fieldName,
            };
            domain.BeginUserEdit(authentication, location);
        }

        public static void RemoveRow(this IDomain domain, Authentication authentication, string tableName, object[] keys)
        {
            var row = new DomainRowInfo()
            {
                TableName = tableName,
                Keys = keys,
            };
            domain.RemoveRow(authentication, new DomainRowInfo[] { row });
        }

        public static void RemoveRow(this IDomain domain, Authentication authentication, object item)
        {
            var row = new DomainRowInfo()
            {
                TableName = CremaDataRowUtility.GetTableName(item),
                Keys = CremaDataRowUtility.GetKeys(item),
            };
            domain.RemoveRow(authentication, new DomainRowInfo[] { row });
        }

        public static void RemoveRows(this IDomain domain, Authentication authentication, IEnumerable items)
        {
            var query = from object item in items
                        select new DomainRowInfo()
                        {
                            TableName = CremaDataRowUtility.GetTableName(item),
                            Keys = CremaDataRowUtility.GetKeys(item),
                        };

            domain.RemoveRow(authentication, query.ToArray());
        }

        public static void SetLocation(this IDomain domain, Authentication authentication, object item, string fieldName)
        {
            var location = new DomainLocationInfo()
            {
                TableName = CremaDataRowUtility.GetTableName(item),
                Keys = CremaDataRowUtility.GetKeys(item),
                ColumnName = fieldName,
            };
            domain.SetUserLocation(authentication, location);
        }

        public static void SetLocation(this IDomain domain, Authentication authentication)
        {
            domain.SetUserLocation(authentication, DomainLocationInfo.Empty);
        }

        public static void SetRow(this IDomain domain, Authentication authentication, string tableName, object[] keys, object[] fields)
        {
            var rowValue = new DomainRowInfo()
            {
                TableName = tableName,
                Keys = keys,
                Fields = fields,
            };
            domain.SetRow(authentication, new DomainRowInfo[] { rowValue });
        }

        public static void SetRow(this IDomain domain, Authentication authentication, object item, string fieldName, object value)
        {
            var rowValue = new DomainRowInfo()
            {
                TableName = CremaDataRowUtility.GetTableName(item),
                Keys = CremaDataRowUtility.GetKeys(item),
                Fields = CremaDataRowUtility.GetFields(item, fieldName, value),
            };
            domain.SetRow(authentication, new DomainRowInfo[] { rowValue });
        }
    }
}
