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
    class EndTableTemplateEditMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public EndTableTemplateEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string>(this.EndTableTemplateEdit);
        }

        [ReturnParameterName("tableName")]
        private string EndTableTemplateEdit(string domainID)
        {
            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return template.Dispatcher.Invoke(() =>
            {
                template.EndEdit(authentication);
                return template.TableName;
            });
        }
    }
}
