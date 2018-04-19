using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class EndTypeEditMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public EndTypeEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string>(this.EndTypeEdit);
        }

        [ReturnParameterName("typeName")]
        private string EndTypeEdit(string domainID)
        {
            if (domainID == null)
                throw new ArgumentNullException(nameof(domainID));

            if (this.CremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                if (Guid.TryParse(domainID, out var guid) == true)
                {
                    var domain = domainContext.Dispatcher.Invoke(() => domainContext.Domains[guid]);
                    if (domain.Host is ITypeTemplate template)
                    {
                        var authentication = this.Context.GetAuthentication(this);
                        return template.Dispatcher.Invoke(() =>
                        {
                            template.EndEdit(authentication);
                            return template.TypeName;
                        });
                    }
                }
                throw new ArgumentException("Invalid domainID", nameof(domainID));
            }
            throw new NotImplementedException();
        }
    }
}
