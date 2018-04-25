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
    class GetTableTemplatePropertiesMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableTemplatePropertiesMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, IDictionary<string, object>>(this.GetTableTemplateProperties);
        }

        private IDictionary<string, object> GetTableTemplateProperties(string domainID, string columnName)
        {
            var template = this.GetDomainHost<ITableTemplate>(domainID);
            return template.Dispatcher.Invoke(() =>
            {
                return new Dictionary<string, object>()
                {
                    { nameof(TableProperties.TableName), template.TableName },
                    { nameof(TableProperties.Tags), (string)template.Tags },
                    { nameof(TableProperties.Comment), template.Comment },
                };
            });
        }
    }
}
