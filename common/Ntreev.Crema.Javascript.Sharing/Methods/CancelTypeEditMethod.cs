using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class CancelTypeEditMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public CancelTypeEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string>(this.CancelTypeEdit);
        }

        private void CancelTypeEdit(string domainID)
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
                        template.Dispatcher.Invoke(() => template.CancelEdit(authentication));
                        return;
                    }
                }
                throw new ArgumentException("Invalid domainID", nameof(domainID));
            }
            throw new NotImplementedException();
        }
    }
}
