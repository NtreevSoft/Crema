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
    class GetTypeTemplatePropertyMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTypeTemplatePropertyMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, TypeProperties, object>(this.GetTypeTemplateProperty);
        }

        private object GetTypeTemplateProperty(string domainID, TypeProperties propertyName)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            return template.Dispatcher.Invoke(() =>
            {
                if (propertyName == TypeProperties.Name)
                    return (object)template.TypeName;
                else if (propertyName == TypeProperties.IsFlag)
                    return (object)template.IsFlag;
                else if (propertyName == TypeProperties.Comment)
                    return (object)template.Comment;

                throw new NotImplementedException();
            });
        }
    }
}
