using Ntreev.Crema.Data.Xml.Schema;
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
    class ContainsTypeTemplateMemberMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public ContainsTypeTemplateMemberMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, bool>(this.ContainsTypeTemplateMember);
        }

        private bool ContainsTypeTemplateMember(string domainID, string memberName)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            return template.Dispatcher.Invoke(() => template.Contains(memberName));
        }
    }
}
