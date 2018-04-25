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
    class EnterTableContentEditMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public EnterTableContentEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string>(this.EnterTableContentEdit);
        }

        private void EnterTableContentEdit(string domainID)
        {
            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            content.Dispatcher.Invoke(() => content.EnterEdit(authentication));
        }
    }
}
