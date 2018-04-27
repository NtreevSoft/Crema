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
    class GetTypeTemplateDomainMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public GetTypeTemplateDomainMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.GetTypeTemplateDomain);
        }

        [ReturnParameterName("domainID")]
        private string GetTypeTemplateDomain(string dataBaseName, string typeName)
        {
            var type = this.GetType(dataBaseName, typeName);
            return type.Dispatcher.Invoke(() =>
            {
                if (type.Template.Domain != null)
                    return $"{type.Template.Domain.ID}";
                return null;
            });
        }
    }
}
