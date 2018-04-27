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
using Ntreev.Crema.Services;
using Ntreev.Library;
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
    class SetTableTemplateColumnPropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public SetTableTemplateColumnPropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string, TableColumnProperties, object>(this.SetTableTemplateColumnProperty);
        }

        private void SetTableTemplateColumnProperty(string domainID, string columnName, TableColumnProperties propertyName, object value)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                var column = template[columnName];
                if (column == null)
                    throw new ItemNotFoundException(columnName);
                if (propertyName == TableColumnProperties.Index)
                {
                    column.SetIndex(authentication, Convert.ToInt32(value));
                }
                else if (propertyName == TableColumnProperties.IsKey)
                {
                    column.SetIsKey(authentication, (bool)value);
                }
                else if (propertyName == TableColumnProperties.IsUnique)
                {
                    column.SetIsUnique(authentication, (bool)value);
                }
                else if (propertyName == TableColumnProperties.Name)
                {
                    column.SetName(authentication, (string)value);
                }
                else if (propertyName == TableColumnProperties.DataType)
                {
                    column.SetDataType(authentication, (string)value);
                }
                else if (propertyName == TableColumnProperties.DefaultValue)
                {
                    column.SetDefaultValue(authentication, $"{value}");
                }
                else if (propertyName == TableColumnProperties.Comment)
                {
                    column.SetComment(authentication, (string)value);
                }
                else if (propertyName == TableColumnProperties.AutoIncrement)
                {
                    column.SetAutoIncrement(authentication, (bool)value);
                }
                else if (propertyName == TableColumnProperties.Tags)
                {
                    column.SetTags(authentication, (TagInfo)(string)value);
                }
                else if (propertyName == TableColumnProperties.IsReadOnly)
                {
                    column.SetIsReadOnly(authentication, (bool)value);
                }
                else if (propertyName == TableColumnProperties.AllowNull)
                {
                    column.SetAllowNull(authentication, (bool)value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }
    }
}
