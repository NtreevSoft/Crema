using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
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
    class GetTableTemplateColumnPropertiesMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableTemplateColumnPropertiesMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, IDictionary<string, object>>(this.GetTableTemplateColumnProperties);
        }

        private IDictionary<string, object> GetTableTemplateColumnProperties(string domainID, string columnName)
        {
            var template = this.GetDomainHost<ITableTemplate>(domainID);
            return template.Dispatcher.Invoke(() =>
            {
                var column = template[columnName];
                if (column == null)
                    throw new ItemNotFoundException(columnName);
                return new Dictionary<string, object>()
                {
                    { nameof(TableColumnProperties.Index), column.Index },
                    { nameof(TableColumnProperties.IsKey), column.IsKey },
                    { nameof(TableColumnProperties.IsUnique), column.IsUnique },
                    { nameof(TableColumnProperties.Name), column.Name },
                    { nameof(TableColumnProperties.DataType), column.DataType },
                    { nameof(TableColumnProperties.DefaultValue), column.DefaultValue },
                    { nameof(TableColumnProperties.Comment), column.Comment },
                    { nameof(TableColumnProperties.AutoIncrement), column.AutoIncrement },
                    { nameof(TableColumnProperties.Tags), column.Tags },
                    { nameof(TableColumnProperties.IsReadOnly), column.IsReadOnly },
                    { nameof(TableColumnProperties.AllowNull), column.AllowNull },
                };
            });
        }
    }
}
