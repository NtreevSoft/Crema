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
    class GetTypeTemplatePropertiesMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTypeTemplatePropertiesMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, IDictionary<string, object>>(this.GetTypeTemplateProperties);
        }

        private IDictionary<string, object> GetTypeTemplateProperties(string domainID)
        {
            var template = this.GetDomainHost<ITypeTemplate>(domainID);
            return template.Dispatcher.Invoke(() =>
            {
                return new Dictionary<string, object>()
                {
                    { nameof(TypeProperties.Name), template.TypeName },
                    { nameof(TypeProperties.IsFlag), template.IsFlag },
                    { nameof(TypeProperties.Comment), template.Comment },
                };
            });
        }
    }
}
