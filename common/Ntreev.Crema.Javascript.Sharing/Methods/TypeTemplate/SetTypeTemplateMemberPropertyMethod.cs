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
    class SetTypeTemplateMemberPropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public SetTypeTemplateMemberPropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string, TypeMemberProperties, object>(this.SetTypeMemberTemplateProperty);
        }

        private void SetTypeMemberTemplateProperty(string domainID, string memberName, TypeMemberProperties propertyName, object value)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                var member = template[memberName];
                if (member == null)
                    throw new ItemNotFoundException(memberName);
                if (propertyName == TypeMemberProperties.Name)
                {
                    member.SetName(authentication, (string)value);
                }
                else if (propertyName == TypeMemberProperties.Value)
                {
                    member.SetValue(authentication, Convert.ToInt64(value));
                }
                else if (propertyName == TypeMemberProperties.Comment)
                {
                    member.SetComment(authentication, (string)value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }
    }
}
