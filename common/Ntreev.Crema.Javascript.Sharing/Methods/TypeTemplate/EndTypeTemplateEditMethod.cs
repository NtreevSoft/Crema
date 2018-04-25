using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TypeTemplate
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TypeTemplate))]
    class EndTypeTemplateEditMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public EndTypeTemplateEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string>(this.EndTypeTemplateEdit);
        }

        [ReturnParameterName("typeName")]
        private string EndTypeTemplateEdit(string domainID)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return template.Dispatcher.Invoke(() =>
            {
                template.EndEdit(authentication);
                return template.TypeName;
            });
        }
    }
}
