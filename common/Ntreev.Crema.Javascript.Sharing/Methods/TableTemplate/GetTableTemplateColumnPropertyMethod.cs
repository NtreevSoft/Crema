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
    class GetTableTemplateColumnPropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableTemplateColumnPropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, TableColumnProperties, object>(this.GetTableTemplateColumnProperty);
        }

        private object GetTableTemplateColumnProperty(string domainID, string columnName, TableColumnProperties propertyName)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return template.Dispatcher.Invoke(() =>
            {
                var column = template[columnName];
                if (column == null)
                    throw new ItemNotFoundException(columnName);
                if (propertyName == TableColumnProperties.Index)
                {
                    return (object)column.Index;
                }
                else if (propertyName == TableColumnProperties.IsKey)
                {
                    return (object)column.IsKey;
                }
                else if (propertyName == TableColumnProperties.IsUnique)
                {
                    return (object)column.IsUnique;
                }
                else if (propertyName == TableColumnProperties.Name)
                {
                    return (object)column.Name;
                }
                else if (propertyName == TableColumnProperties.DataType)
                {
                    return (object)column.DataType;
                }
                else if (propertyName == TableColumnProperties.DefaultValue)
                {
                    return (object)column.DefaultValue;
                }
                else if (propertyName == TableColumnProperties.Comment)
                {
                    return (object)column.Comment;
                }
                else if (propertyName == TableColumnProperties.AutoIncrement)
                {
                    return (object)column.AutoIncrement;
                }
                else if (propertyName == TableColumnProperties.Tags)
                {
                    return (object)(string)column.Tags;
                }
                else if (propertyName == TableColumnProperties.IsReadOnly)
                {
                    return (object)column.IsReadOnly;
                }
                else if (propertyName == TableColumnProperties.AllowNull)
                {
                    return (object)column.AllowNull;
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }
    }
}
