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
    class ContainsTableContentRowMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public ContainsTableContentRowMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, object[], bool>(this.ContainsTableContentRow);
        }

        private bool ContainsTableContentRow(string domainID, object[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return content.Dispatcher.Invoke(() => content.Find(authentication, keys) != null);
        }
    }
}
