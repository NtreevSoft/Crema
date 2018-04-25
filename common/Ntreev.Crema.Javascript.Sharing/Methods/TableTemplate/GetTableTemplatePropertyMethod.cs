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
    class GetTableTemplatePropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableTemplatePropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, TableProperties, object>(this.GetTableTemplateProperty);
        }

        private object GetTableTemplateProperty(string domainID, string columnName, TableProperties propertyName)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return template.Dispatcher.Invoke(() =>
            {
                if (propertyName == TableProperties.TableName)
                {
                    return (object)template.TableName;
                }
                else if (propertyName == TableProperties.Tags)
                {
                    return (object)(string)template.Tags;
                }
                else if (propertyName == TableProperties.Comment)
                {
                    return (object)template.Comment;
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }
    }
}
