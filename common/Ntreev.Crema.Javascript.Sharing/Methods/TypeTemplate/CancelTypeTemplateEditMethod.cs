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
    class CancelTypeTemplateEditMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public CancelTypeTemplateEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string>(this.CancelTypeTemplateEdit);
        }

        private void CancelTypeTemplateEdit(string domainID)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() => template.CancelEdit(authentication));
        }
    }
}
