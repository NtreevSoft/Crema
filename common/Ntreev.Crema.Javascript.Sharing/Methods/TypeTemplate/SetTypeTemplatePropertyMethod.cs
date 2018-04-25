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
    class SetTypeTemplatePropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public SetTypeTemplatePropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, TypeProperties, object>(this.SetTypeTemplateProperty);
        }

        private void SetTypeTemplateProperty(string domainID, TypeProperties propertyName, object value)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                if (propertyName == TypeProperties.Name)
                {
                    template.SetTypeName(authentication, (string)value);
                }
                else if (propertyName == TypeProperties.IsFlag)
                {
                    template.SetIsFlag(authentication, (bool)value);
                }
                else if (propertyName == TypeProperties.Comment)
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
