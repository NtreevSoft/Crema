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
    class AddTypeTemplateMemberMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public AddTypeTemplateMemberMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string, long, string>(this.AddTypeTemplateMember);
        }

        private void AddTypeTemplateMember(string domainID, string memberName, long value, string comment)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                var typeMember = template.AddNew(authentication);
                typeMember.SetName(authentication, memberName);
                typeMember.SetValue(authentication, value);
                if (comment != null)
                    typeMember.SetComment(authentication, comment);
                template.EndNew(authentication, typeMember);
            });
        }
    }
}
