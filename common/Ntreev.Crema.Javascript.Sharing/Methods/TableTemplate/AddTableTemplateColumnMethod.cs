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

using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableTemplate
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableTemplate))]
    class AddTableTemplateColumnMethod : DomainScriptMethodBase
    {
        private const string typeNameDescription = "boolean, string, int, float, double, dateTime, unsignedInt, long, short, unsignedLong, unsignedByte, duration, unsignedShort, byte, guid or typePath(e.g., /categoryPath/typeName)";

        [ImportingConstructor]
        public AddTableTemplateColumnMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string, string, string, bool?>(this.AddTableTemplateColumn);
        }

        private void AddTableTemplateColumn(string domainID, string columnName,
            [Description(typeNameDescription)]
            string typeName, string comment, bool? isKey)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                var column = template.AddNew(authentication);
                column.SetName(authentication, columnName);
                column.SetDataType(authentication, typeName);
                if (comment != null)
                    column.SetComment(authentication, comment);
                if (isKey != null)
                    column.SetIsKey(authentication, isKey.Value);
                template.EndNew(authentication, column);
            });
        }
    }
}
