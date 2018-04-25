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
    class GetTypeTemplateMemberPropertiesMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTypeTemplateMemberPropertiesMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, IDictionary<string, object>>(this.GetTypeTemplateMemberProperties);
        }

        private IDictionary<string, object> GetTypeTemplateMemberProperties(string domainID, string memberName)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            return template.Dispatcher.Invoke(() =>
            {
                var member = template[memberName];
                if (member == null)
                    throw new ItemNotFoundException(memberName);
                return new Dictionary<string, object>()
                {
                    { nameof(TypeMemberProperties.Name), member.Name },
                    { nameof(TypeMemberProperties.Value), member.Value },
                    { nameof(TypeMemberProperties.Comment), member.Comment },
                };
            });
        }
    }
}
