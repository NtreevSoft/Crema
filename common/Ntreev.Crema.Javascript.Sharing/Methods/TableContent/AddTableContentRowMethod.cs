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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableContent
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableContent))]
    class AddTableContentRowMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public AddTableContentRowMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, IDictionary<string, object>, object[]>(this.AddTableContentRow);
        }

        [ReturnParameterName("keys")]
        private object[] AddTableContentRow(string domainID, string tableName, IDictionary<string, object> fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            var contents = this.GetDomainHost<IEnumerable<ITableContent>>(domainID);
            var content = contents.FirstOrDefault(item => item.Dispatcher.Invoke(() => item.Table.Name) == tableName);
            if (content == null)
                throw new TableNotFoundException(tableName);
            var authentication = this.Context.GetAuthentication(this);
            return content.Dispatcher.Invoke(() =>
            {
                var tableInfo = content.Table.TableInfo;
                var row = content.AddNew(authentication, null);
                foreach (var item in fields)
                {
                    var typeName = tableInfo.Columns.First(i => i.DataType == item.Key).DataType;
                    var type = CremaDataTypeUtility.GetType(typeName);
                    var value = CremaConvert.ChangeType(item.Value, type);
                    row.SetField(authentication, item.Key, value);
                }
                content.EndNew(authentication, row);
                return tableInfo.Columns.Select(item => row[item.Name]).ToArray();
            });
        }
    }
}
