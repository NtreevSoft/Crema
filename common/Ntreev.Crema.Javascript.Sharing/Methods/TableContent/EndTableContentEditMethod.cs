using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableContent
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableContent))]
    class EndTableContentEditMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public EndTableContentEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string>(this.EndTableContentEdit);
        }

        private void EndTableContentEdit(string domainID)
        {
            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            content.Dispatcher.Invoke(() => content.EndEdit(authentication));
        }
    }
}
