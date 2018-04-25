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
    class SetTableTemplatePropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public SetTableTemplatePropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string, TableProperties, object>(this.SetTableTemplateProperty);
        }

        private void SetTableTemplateProperty(string domainID, string columnName, TableProperties propertyName, object value)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                if (propertyName == TableProperties.TableName)
                {
                    template.SetTableName(authentication, (string)value);
                }
                else if (propertyName == TableProperties.Tags)
                {
                    template.SetTags(authentication, (TagInfo)(string)value);
                }
                else if (propertyName == TableProperties.Comment)
                {
                    template.SetComment(authentication, (string)value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }
    }
}
