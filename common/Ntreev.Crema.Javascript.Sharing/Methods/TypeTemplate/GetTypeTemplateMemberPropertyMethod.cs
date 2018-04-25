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
    class GetTypeTemplateMemberPropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTypeTemplateMemberPropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, TypeMemberProperties, object>(this.GetTypeTemplateMemberProperty);
        }

        private object GetTypeTemplateMemberProperty(string domainID, string memberName, TypeMemberProperties propertyName)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            return template.Dispatcher.Invoke(() =>
            {
                var member = template[memberName];
                if (member == null)
                    throw new ItemNotFoundException(memberName);
                if (propertyName == TypeMemberProperties.Name)
                    return (object)member.Name;
                else if (propertyName == TypeMemberProperties.Value)
                    return (object)member.Value;
                else if (propertyName == TypeMemberProperties.Comment)
                    return (object)member.Comment;

                throw new NotImplementedException();
            });
        }
    }
}
